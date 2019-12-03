using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Values;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Components
{
    public class ActorComponent : Component
    {
        public event Payload<ActorComponent> Hidden;
        public event Payload<ActorComponent> Shown;
        public event Payload<ActorComponent> ModelChanged;
        public event Payload<string> AnimationPlayed;

        public bool IsVisible => Model.IsVisible;
        public bool IsHelmVisible { get; private set; } = true;

        public Model Model { get; private set; }
        public Model OriginalModel { get; private set; }

        private readonly Dictionary<object, List<Tuple<AttachmentPoint, GameObject>>> attachments
            = new Dictionary<object, List<Tuple<AttachmentPoint, GameObject>>>();

        private string lastPlayedAnimation = "idle";

        public ActorComponent Construct(Model model)
        {
            Model = model;
            Model.Initialize();

            OriginalModel = model;

            return this;
        }

        protected override void OnInitialize()
        {
            var behaviours = GetComponent<BehavioursComponent>();
            behaviours.BehaviourApplied += OnBehaviourApplied;
            behaviours.BehaviourRemoved += OnBehaviourRemoved;

            var health = GetComponent<HealthComponent>();
            health.Died += OnDied;
            health.Damaged += OnDamaged;
        }

        protected override void OnTerminate()
        {
            var behaviours = GetComponent<BehavioursComponent>();
            behaviours.BehaviourApplied -= OnBehaviourApplied;
            behaviours.BehaviourRemoved -= OnBehaviourRemoved;

            var health = GetComponent<HealthComponent>();
            health.Died -= OnDied;
            health.Damaged -= OnDamaged;
        }

        public void ChangeModel(string model)
        {
            var prefab = Resources.Load<Model>(model);

            if (prefab == null)
            {
                Debug.LogError($"No model found at path {model}");
                return;
            }

            OriginalModel.Hide();

            Model = Instantiate(
                prefab,
                OriginalModel.transform.position,
                OriginalModel.transform.rotation,
                transform
            );

            Model.Initialize();
            Model.PlayAnimation(this.lastPlayedAnimation);

            ModelChanged?.Invoke(this);
        }

        public void RestoreModel()
        {
            if (OriginalModel == null || Model == OriginalModel)
            {
                return;
            }

            var wasVisible = Model.IsVisible;

            Model.Hide();

            // Wait until floating combat text destroys.
            Destroy(Model.gameObject, 5);

            Model = OriginalModel;

            if (wasVisible)
            {
                Model.Show();
            }
            else
            {
                Model.Hide();
            }

            PlayAnimation(this.lastPlayedAnimation);

            ModelChanged?.Invoke(this);
        }

        public void CreateAttachments(object source, List<AttachmentInfo> attachments,
            AttachmentPoint point = AttachmentPoint.None)
        {
            if (!this.attachments.ContainsKey(source))
            {
                this.attachments.Add(source, new List<Tuple<AttachmentPoint, GameObject>>());
            }

            foreach (var attachmentInfo in attachments)
            {
                var prefab = Resources.Load<GameObject>(attachmentInfo.Prefab);

                if (prefab == null)
                {
                    Debug.LogError($"Prefab not found at {attachmentInfo.Prefab}");
                    return;
                }

                var attachmentPoint = attachmentInfo.Point == AttachmentPoint.Auto ? point : attachmentInfo.Point;
                var attachmentPointTransform = Model.GetAttachmentPoint(attachmentPoint);

                var attachment = Instantiate(prefab, attachmentPointTransform, attachmentInfo.OriginalScale);
                attachment.transform.localPosition = Vector3.zero;
                attachment.transform.localRotation = Quaternion.identity;

                foreach (var component in attachment.GetComponentsInChildren<Transform>())
                {
                    component.gameObject.layer = Model.gameObject.layer;
                }

                this.attachments[source].Add(new Tuple<AttachmentPoint, GameObject>(attachmentPoint, attachment));
            }

            Model.OnAttachmentAdded();
        }

        public void SetHelmVisible(bool isVisible)
        {
            IsHelmVisible = isVisible;

            if (isVisible)
            {
                Model.ShowHelm();

                if (this.attachments.Values.Any(list => list.Any(t => t.Item1 == AttachmentPoint.Head)))
                {
                    Model.HideHair();
                }
            }
            else
            {
                Model.HideHelm();
                Model.ShowHair();
            }
        }

        public void DestroyAttachments(object source)
        {
            if (!this.attachments.ContainsKey(source))
            {
                return;
            }

            foreach (var (point, attachment) in this.attachments[source])
            {
                if (attachment == null)
                {
                    continue;
                }

                attachment.DestroyAsVisualEffect();
            }

            this.attachments.Remove(source);
        }

        public void PlayAnimation(string animation)
        {
            AnimationPlayed?.Invoke(animation);

            this.lastPlayedAnimation = animation;
            Model.PlayAnimation(animation);

            // Debug.Log($"Playing animation \"{animation}\" on {name}'s actor.");
        }

        public void Show()
        {
            Model.Show();
            Shown?.Invoke(this);
        }

        public void Hide()
        {
            Model.Hide();
            Hidden?.Invoke(this);
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (!behaviour.StatusFlags.HasFlag(StatusFlags.Invisibility))
            {
                return;
            }

            Model.ChangeTransparency(0.25f);
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            if (GetComponent<BehavioursComponent>().IsInvisible)
            {
                return;
            }

            Model.ChangeTransparency(1.0f);
        }

        private void OnDamaged(EntityDamagedEventData data)
        {
            var movement = GetComponent<MovementComponent>();

            if (movement != null && movement.IsMoving)
            {
                return;
            }

            if (data.Damage.IsBlocked())
            {
                AudioManager.Instance.PlayBlock();
                PlayAnimation("block");
                return;
            }

            if (data.Damage.IsDodged())
            {
                AudioManager.Instance.PlayDodge();
                PlayAnimation("dodge");
                return;
            }

            PlayAnimation("hit");
        }

        private void OnDied(EntityDiedEventData data)
        {
            if (gameObject.IsCharacter())
            {
                PlayAnimation("death");
                return;
            }

            if (data.Damage.IsCritical() && RNG.Test(0.25f) &&
                !data.Damage.IsDOT() &&
                !data.Victim.IsSummoned() &&
                !data.Victim.IsCorpseless())
            {
                var vfx = GetDeathVisualEffect(data.Damage);

                if (vfx != null)
                {
                    Model.Hide();
                    Instantiate(vfx, transform.position, Quaternion.identity).DestroyAsVisualEffect();
                    return;
                }
            }

            PlayAnimation("death");
            StartCoroutine(Timer.Coroutine(new WaitForSeconds(4), () => Model.FadeOut(1)));
            StartCoroutine(Timer.Coroutine(new WaitForSeconds(6), () => gameObject.SetActive(false)));
        }

        private static GameObject GetDeathVisualEffect(Damage damage)
        {
            if (damage.IsPhysicalType() || damage.Type == DamageType.Chaos)
            {
                return Resources.Load<GameObject>("Prefabs/VFX/Death/Death_Blood");
            }

            if (damage.Type == DamageType.Fire)
            {
                return Resources.Load<GameObject>("Prefabs/VFX/Death/Death_Fire");
            }

            if (damage.Type == DamageType.Cold)
            {
                return Resources.Load<GameObject>("Prefabs/VFX/Death/Death_Ice");
            }

            if (damage.Type == DamageType.Lightning)
            {
                return Resources.Load<GameObject>("Prefabs/VFX/Death/Death_Lightning");
            }

            if (damage.Type == DamageType.Poison)
            {
                return Resources.Load<GameObject>("Prefabs/VFX/Death/Death_Poison");
            }

            if (damage.Type == DamageType.Shadow || damage.Type == DamageType.Arcane)
            {
                return Resources.Load<GameObject>("Prefabs/VFX/Death/Death_Shadow");
            }

            return null;
        }
    }
}