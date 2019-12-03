using System;
using System.Collections.Generic;
using Anima2D;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary
{
    [RequireComponent(typeof(Animator))]
    public class Model : MonoBehaviour
    {
        public bool IsVisible { get; private set; } = true;
        public bool IsFacingLeft { get; private set; } = true;

        [Header("Attachment Points")]
        [SerializeField] private Transform headAttachmentPoint;
        [SerializeField] private Transform hairAttachmentPoint;
        [SerializeField] private Transform overheadAttachmentPoint;
        [SerializeField] private Transform chestAttachmentPoint;
        [SerializeField] private Transform beltAttachmentPoint;
        [SerializeField] private Transform gunBarrel;
        [SerializeField] private Transform rightHandAttachmentPoint;
        [SerializeField] private Transform leftHandAttachmentPoint;
        [SerializeField] private Transform rightFistAttachmentPoint;
        [SerializeField] private Transform leftFistAttachmentPoint;
        [SerializeField] private Transform leftShoulderAttachmentPoint;
        [SerializeField] private Transform rightShoulderAttachmentPoint;
        [SerializeField] private Transform leftFootAttachmentPoint;
        [SerializeField] private Transform rightFootAttachmentPoint;
        [SerializeField] private Transform shieldAttachmentPoint;
        [SerializeField] private Transform originAttachmentPoint;
        [SerializeField] private Transform backAttachmentPoint;
        [SerializeField] private Transform special01AttachmentPoint;
        [SerializeField] private Transform special02AttachmentPoint;
        [SerializeField] private Transform special03AttachmentPoint;

        [Header("Equipment Meshes")]
        [SerializeField] private SpriteMeshInstance kiltMesh;
        [SerializeField] private SpriteMeshInstance bootsLeftMesh;
        [SerializeField] private SpriteMeshInstance bootsRightMesh;
        [SerializeField] private SpriteMeshInstance glovesLeftMesh;
        [SerializeField] private SpriteMeshInstance glovesRightMesh;
        [SerializeField] private SpriteMeshInstance armorCenterMesh;
        [SerializeField] private SpriteMeshInstance armorLeft01Mesh;
        [SerializeField] private SpriteMeshInstance armorLeft02Mesh;
        [SerializeField] private SpriteMeshInstance armorLeft03Mesh;
        [SerializeField] private SpriteMeshInstance armorRight01Mesh;
        [SerializeField] private SpriteMeshInstance armorRight02Mesh;
        [SerializeField] private SpriteMeshInstance armorRight03Mesh;
        [SerializeField] private SpriteMeshInstance pantsCenterMesh;
        [SerializeField] private SpriteMeshInstance pantsLeft01Mesh;
        [SerializeField] private SpriteMeshInstance pantsLeft02Mesh;
        [SerializeField] private SpriteMeshInstance pantsRight01Mesh;
        [SerializeField] private SpriteMeshInstance pantsRight02Mesh;

        [Space(20)]
        [SerializeField] private SpriteGroup skin;
        [SerializeField] private SpriteGroup hairstyle;
        [SerializeField] private SpriteGroup beard;
        [SerializeField] private SpriteMesh emptySpriteMesh;

        [Space(20)]
        [SerializeField] private GameObject pistolMuzzleFlash;
        [SerializeField] private GameObject rifleMuzzleFlash;
        [SerializeField] private GameObject shotgunMuzzleFlash;
        [SerializeField] private GameObject blockParticles;

        private readonly Dictionary<SpriteMeshInstance, Color> originalColors = new Dictionary<SpriteMeshInstance, Color>();

        private Dictionary<AttachmentPoint, Transform> attachmentPoints;
        private Dictionary<SkinPart, SpriteMeshInstance> bodyParts;

        private Animator animator;
        private bool isVisible;
        private bool isInitialized;
        private Vector3 originalScale;

        private void Start()
        {
            Initialize();
            FadeIn();
        }

        public void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            this.originalScale = transform.localScale;

            this.animator = GetComponent<Animator>();

            this.attachmentPoints = new Dictionary<AttachmentPoint, Transform>
            {
                {AttachmentPoint.Head, this.headAttachmentPoint},
                {AttachmentPoint.Hair, this.hairAttachmentPoint},
                {AttachmentPoint.OverHead, this.overheadAttachmentPoint},
                {AttachmentPoint.Chest, this.chestAttachmentPoint},
                {AttachmentPoint.GunBarrel, this.gunBarrel},
                {AttachmentPoint.RightHand, this.rightHandAttachmentPoint},
                {AttachmentPoint.LeftHand, this.leftHandAttachmentPoint},
                {AttachmentPoint.RightFist, this.rightFistAttachmentPoint},
                {AttachmentPoint.LeftFist, this.leftFistAttachmentPoint},
                {AttachmentPoint.RightFoot, this.rightFootAttachmentPoint},
                {AttachmentPoint.LeftFoot, this.leftFootAttachmentPoint},
                {AttachmentPoint.Origin, this.originAttachmentPoint},
                {AttachmentPoint.Shield, this.shieldAttachmentPoint},
                {AttachmentPoint.Belt, this.beltAttachmentPoint},
                {AttachmentPoint.LeftShoulder, this.leftShoulderAttachmentPoint},
                {AttachmentPoint.RightShoulder, this.rightShoulderAttachmentPoint},
                {AttachmentPoint.Back, this.backAttachmentPoint},
                {AttachmentPoint.Special01, this.special01AttachmentPoint},
                {AttachmentPoint.Special02, this.special02AttachmentPoint},
                {AttachmentPoint.Special03, this.special03AttachmentPoint},
                {AttachmentPoint.None, transform},
            };

            this.bodyParts = new Dictionary<SkinPart, SpriteMeshInstance>
            {
                {SkinPart.Kilt, this.kiltMesh},
                {SkinPart.BootsLeft, this.bootsLeftMesh},
                {SkinPart.BootsRight, this.bootsRightMesh},
                {SkinPart.GlovesLeft, this.glovesLeftMesh},
                {SkinPart.GlovesRight, this.glovesRightMesh},
                {SkinPart.ArmorCenter, this.armorCenterMesh},
                {SkinPart.ArmorLeft01, this.armorLeft01Mesh},
                {SkinPart.ArmorLeft02, this.armorLeft02Mesh},
                {SkinPart.ArmorLeft03, this.armorLeft03Mesh},
                {SkinPart.ArmorRight01, this.armorRight01Mesh},
                {SkinPart.ArmorRight02, this.armorRight02Mesh},
                {SkinPart.ArmorRight03, this.armorRight03Mesh},
                {SkinPart.PantsCenter, this.pantsCenterMesh},
                {SkinPart.PantsLeft01, this.pantsLeft01Mesh},
                {SkinPart.PantsLeft02, this.pantsLeft02Mesh},
                {SkinPart.PantsRight01, this.pantsRight01Mesh},
                {SkinPart.PantsRight02, this.pantsRight02Mesh},
            };

            this.isInitialized = true;
        }

        public void PlayFootsteps()
        {
            AudioManager.Instance.PlayFootsteps(transform.position);
        }

        public void PlayDig()
        {
            AudioManager.Instance.PlayDig(transform.position);
        }

        public void CreateRifleMuzzleFlash()
        {
            Timer.Instance.Wait(0.15f, () => Instantiate(this.rifleMuzzleFlash, this.gunBarrel).DestroyAsVisualEffect());
        }

        public void CreateShotgunMuzzleFlash()
        {
            Timer.Instance.Wait(0.15f, () => Instantiate(this.shotgunMuzzleFlash, this.gunBarrel).DestroyAsVisualEffect());
        }

        public void CreatePistolMuzzleFlash()
        {
            Timer.Instance.Wait(0.15f, () => Instantiate(this.pistolMuzzleFlash, this.gunBarrel).DestroyAsVisualEffect());
        }

        public void CreateBlockParticles()
        {
            Timer.Instance.Wait(0.15f, () =>
                Instantiate(this.blockParticles, this.shieldAttachmentPoint.transform.position, Quaternion.identity)
                    .DestroyAsVisualEffect());
        }

        public void PlayAnimation(string animation)
        {
            if (this.animator == null)
            {
                return;
            }

            this.animator.Play(animation, -1, 0);
        }

        public float GetAnimationLength(string name)
        {
            if (this.animator == null)
            {
                return 0;
            }

            return this.animator.GetAnimationClipLength(name);
        }

        public Transform GetAttachmentPoint(AttachmentPoint attachmentPoint)
        {
            if (this.attachmentPoints[attachmentPoint] != null)
            {
                return this.attachmentPoints[attachmentPoint];
            }

            Debug.LogWarning($"No attachment point of type {attachmentPoint} on actor {name}");
            return transform;
        }

        private SpriteMeshInstance GetEquipmentMesh(SkinPart part)
        {
            return this.bodyParts.GetValueOrDefault(part, null);
        }

        public void ApplySkin(Skin skin)
        {
            foreach (var skinPart in skin.Parts)
            {
                var mesh = Resources.Load<SpriteMesh>(skinPart.Mesh);

                if (mesh == null)
                {
                    continue;
                }

                GetEquipmentMesh(skinPart.Part).spriteMesh = mesh;
            }
        }

        public void RemoveSkin(Skin skin)
        {
            foreach (var skinPart in skin.Parts)
            {
                var mesh = Resources.Load<SpriteMesh>(skinPart.Mesh);

                if (mesh == null)
                {
                    continue;
                }

                GetEquipmentMesh(skinPart.Part).spriteMesh = this.emptySpriteMesh;
            }
        }

        public void ShowHelm()
        {
            this.headAttachmentPoint.gameObject.SetActive(true);
        }

        public void HideHelm()
        {
            this.headAttachmentPoint.gameObject.SetActive(false);
        }

        public void ShowHair()
        {
            this.hairstyle.gameObject.SetActive(true);
        }

        public void HideHair()
        {
            this.hairstyle.gameObject.SetActive(false);
        }

        public void Show()
        {
            SetRenderersEnabled(true);
        }

        public void Hide()
        {
            SetRenderersEnabled(false);
        }

        private void SetRenderersEnabled(bool visible)
        {
            IsVisible = visible;

            foreach (var element in GetComponentsInChildren<Renderer>())
            {
                element.enabled = IsVisible;
            }
        }

        public void OnAttachmentAdded()
        {
            SetRenderersEnabled(IsVisible);
        }

        public void LookAt(Vector3 point)
        {
            var delta = point.x - transform.position.x;

            if (Math.Abs(delta) < 0.5f)
            {
                return;
            }

            IsFacingLeft = delta < 0;

            FlipX(!IsFacingLeft);
        }

        public void FlipX(bool flip)
        {
            transform.localScale = transform.localScale.With(Mathf.Abs(transform.localScale.x));
            transform.rotation = Quaternion.Euler(0, flip ? 180 : 0, 0);
        }

        public void FadeIn(float duration = 1.5f)
        {
            foreach (var mesh in GetComponentsInChildren<SpriteMeshInstance>())
            {
                mesh.FadeIn(duration);
            }

            foreach (var element in GetComponentsInChildren<SpriteRenderer>())
            {
                element.FadeIn(duration);
            }
        }

        public void FadeOut(float duration = 1.5f)
        {
            foreach (var mesh in GetComponentsInChildren<SpriteMeshInstance>())
            {
                mesh.FadeOut(duration);
            }

            foreach (var element in GetComponentsInChildren<SpriteRenderer>())
            {
                element.FadeOut(duration);
            }

            StartCoroutine(Timer.Coroutine(new WaitForSeconds(duration), () =>
            {
                foreach (var particles in GetComponentsInChildren<ParticleSystem>())
                {
                    particles.Stop();
                }
            }));
        }

        public void ChangeHairstyle(SpriteGroup hairstyle)
        {
            hairstyle.gameObject.layer = gameObject.layer;
            hairstyle.transform.SetParent(this.hairstyle.transform.parent);
            hairstyle.transform.localRotation = Quaternion.identity;
            hairstyle.transform.localPosition = Vector3.zero;
            hairstyle.transform.localScale = Vector3.one;
            hairstyle.gameObject.SetActive(this.hairstyle.gameObject.activeSelf);

            Destroy(this.hairstyle.gameObject);

            this.hairstyle = hairstyle;
        }

        public void ChangeBeard(SpriteGroup beard)
        {
            beard.gameObject.layer = gameObject.layer;
            beard.transform.SetParent(this.beard.transform.parent);
            beard.transform.localRotation = Quaternion.identity;
            beard.transform.localPosition = Vector3.zero;
            beard.transform.localScale = Vector3.one;
            beard.gameObject.SetActive(this.beard.gameObject.activeSelf);

            Destroy(this.beard.gameObject);

            this.beard = beard;
        }

        public void ChangeScale(float scale)
        {
            transform.localScale *= scale;
        }

        public void ResetScale()
        {
            transform.localScale = this.originalScale;
        }

        public void ChangeColor(Color color)
        {
            ResetColor();

            foreach (var mesh in GetComponentsInChildren<SpriteMeshInstance>())
            {
                this.originalColors.Add(mesh, mesh.color);
                mesh.color = color;
            }
        }

        public void ResetColor()
        {
            foreach (var pair in this.originalColors)
            {
                if (pair.Key == null)
                {
                    continue;
                }

                pair.Key.color = pair.Value;
            }

            this.originalColors.Clear();
        }

        public void ChangeHairColor(Color color)
        {
            this.hairstyle.ChangeColor(color);
        }

        public void ChangeBeardColor(Color color)
        {
            this.beard.ChangeColor(color);
        }

        public void ChangeSkinColor(Color color)
        {
            this.skin.ChangeColor(color);
        }

        public void ChangeTransparency(float amount)
        {
            foreach (var mesh in GetComponentsInChildren<SpriteMeshInstance>())
            {
                mesh.color = mesh.color.With(a: amount);
            }

            foreach (var element in GetComponentsInChildren<SpriteRenderer>())
            {
                element.color = element.color.With(a: amount);
            }
        }

        public void EnableMasking()
        {
            foreach (var mask in GetComponentsInChildren<SpriteMask>())
            {
                mask.enabled = true;
            }

            foreach (var mesh in GetComponentsInChildren<SpriteMeshInstance>())
            {
                mesh.sortingLayerName = "Actors Masked";
            }
        }

        public void DisableMasking()
        {
            foreach (var mask in GetComponentsInChildren<SpriteMask>())
            {
                mask.enabled = false;
            }

            foreach (var mesh in GetComponentsInChildren<SpriteMeshInstance>())
            {
                mesh.sortingLayerName = "Actors";
            }
        }
    }
}