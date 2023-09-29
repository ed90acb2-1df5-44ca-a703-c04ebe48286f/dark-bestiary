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
        public bool FadeInOnStart { get; set; }

        
        [Header("Attachment Points")]
        [SerializeField] private Transform m_HeadAttachmentPoint;
        [SerializeField] private Transform m_MaskAttachmentPoint;
        [SerializeField] private Transform m_HairAttachmentPoint;
        [SerializeField] private Transform m_OverheadAttachmentPoint;
        [SerializeField] private Transform m_ChestAttachmentPoint;
        [SerializeField] private Transform m_BeltAttachmentPoint;
        [SerializeField] private Transform m_GunBarrel;
        [SerializeField] private Transform m_RightHandAttachmentPoint;
        [SerializeField] private Transform m_LeftHandAttachmentPoint;
        [SerializeField] private Transform m_RightFistAttachmentPoint;
        [SerializeField] private Transform m_LeftFistAttachmentPoint;
        [SerializeField] private Transform m_LeftShoulderAttachmentPoint;
        [SerializeField] private Transform m_RightShoulderAttachmentPoint;
        [SerializeField] private Transform m_LeftFootAttachmentPoint;
        [SerializeField] private Transform m_RightFootAttachmentPoint;
        [SerializeField] private Transform m_ShieldAttachmentPoint;
        [SerializeField] private Transform m_OriginAttachmentPoint;
        [SerializeField] private Transform m_BackAttachmentPoint;
        [SerializeField] private Transform m_Special01AttachmentPoint;
        [SerializeField] private Transform m_Special02AttachmentPoint;
        [SerializeField] private Transform m_Special03AttachmentPoint;

        
        [Header("Equipment Meshes")]
        [SerializeField] private SpriteMeshInstance m_KiltMesh;
        [SerializeField] private SpriteMeshInstance m_BootsLeftMesh;
        [SerializeField] private SpriteMeshInstance m_BootsRightMesh;
        [SerializeField] private SpriteMeshInstance m_GlovesLeftMesh;
        [SerializeField] private SpriteMeshInstance m_GlovesRightMesh;
        [SerializeField] private SpriteMeshInstance m_ArmorCenterMesh;
        [SerializeField] private SpriteMeshInstance m_ArmorLeft01Mesh;
        [SerializeField] private SpriteMeshInstance m_ArmorLeft02Mesh;
        [SerializeField] private SpriteMeshInstance m_ArmorLeft03Mesh;
        [SerializeField] private SpriteMeshInstance m_ArmorRight01Mesh;
        [SerializeField] private SpriteMeshInstance m_ArmorRight02Mesh;
        [SerializeField] private SpriteMeshInstance m_ArmorRight03Mesh;
        [SerializeField] private SpriteMeshInstance m_PantsCenterMesh;
        [SerializeField] private SpriteMeshInstance m_PantsLeft01Mesh;
        [SerializeField] private SpriteMeshInstance m_PantsLeft02Mesh;
        [SerializeField] private SpriteMeshInstance m_PantsRight01Mesh;
        [SerializeField] private SpriteMeshInstance m_PantsRight02Mesh;

        
        [Space(20)]
        [SerializeField] private SpriteGroup m_Skin;
        [SerializeField] private SpriteGroup m_Hairstyle;
        [SerializeField] private SpriteGroup m_Beard;
        [SerializeField] private SpriteMesh m_EmptySpriteMesh;

        
        [Space(20)]
        [SerializeField] private GameObject m_PistolMuzzleFlash;
        [SerializeField] private GameObject m_RifleMuzzleFlash;
        [SerializeField] private GameObject m_ShotgunMuzzleFlash;
        [SerializeField] private GameObject m_BlockParticles;

        private readonly Dictionary<SpriteMeshInstance, Color> m_OriginalColors = new();

        private Dictionary<AttachmentPoint, Transform> m_AttachmentPoints;
        private Dictionary<SkinPart, SpriteMeshInstance> m_BodyParts;

        private Animator m_Animator;
        private bool m_IsVisible;
        private bool m_IsInitialized;
        private Vector3 m_OriginalScale;

        private void Start()
        {
            Initialize();

            if (FadeInOnStart)
            {
                FadeIn();
            }
        }

        public void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }

            m_OriginalScale = transform.localScale;

            m_Animator = GetComponent<Animator>();

            m_AttachmentPoints = new Dictionary<AttachmentPoint, Transform>
            {
                {AttachmentPoint.Head, m_HeadAttachmentPoint},
                {AttachmentPoint.Mask, m_MaskAttachmentPoint},
                {AttachmentPoint.Hair, m_HairAttachmentPoint},
                {AttachmentPoint.OverHead, m_OverheadAttachmentPoint},
                {AttachmentPoint.Chest, m_ChestAttachmentPoint},
                {AttachmentPoint.GunBarrel, m_GunBarrel},
                {AttachmentPoint.RightHand, m_RightHandAttachmentPoint},
                {AttachmentPoint.LeftHand, m_LeftHandAttachmentPoint},
                {AttachmentPoint.RightFist, m_RightFistAttachmentPoint},
                {AttachmentPoint.LeftFist, m_LeftFistAttachmentPoint},
                {AttachmentPoint.RightFoot, m_RightFootAttachmentPoint},
                {AttachmentPoint.LeftFoot, m_LeftFootAttachmentPoint},
                {AttachmentPoint.Origin, m_OriginAttachmentPoint},
                {AttachmentPoint.Shield, m_ShieldAttachmentPoint},
                {AttachmentPoint.Belt, m_BeltAttachmentPoint},
                {AttachmentPoint.LeftShoulder, m_LeftShoulderAttachmentPoint},
                {AttachmentPoint.RightShoulder, m_RightShoulderAttachmentPoint},
                {AttachmentPoint.Back, m_BackAttachmentPoint},
                {AttachmentPoint.Special01, m_Special01AttachmentPoint},
                {AttachmentPoint.Special02, m_Special02AttachmentPoint},
                {AttachmentPoint.Special03, m_Special03AttachmentPoint},
                {AttachmentPoint.None, transform},
            };

            m_BodyParts = new Dictionary<SkinPart, SpriteMeshInstance>
            {
                {SkinPart.Kilt, m_KiltMesh},
                {SkinPart.BootsLeft, m_BootsLeftMesh},
                {SkinPart.BootsRight, m_BootsRightMesh},
                {SkinPart.GlovesLeft, m_GlovesLeftMesh},
                {SkinPart.GlovesRight, m_GlovesRightMesh},
                {SkinPart.ArmorCenter, m_ArmorCenterMesh},
                {SkinPart.ArmorLeft01, m_ArmorLeft01Mesh},
                {SkinPart.ArmorLeft02, m_ArmorLeft02Mesh},
                {SkinPart.ArmorLeft03, m_ArmorLeft03Mesh},
                {SkinPart.ArmorRight01, m_ArmorRight01Mesh},
                {SkinPart.ArmorRight02, m_ArmorRight02Mesh},
                {SkinPart.ArmorRight03, m_ArmorRight03Mesh},
                {SkinPart.PantsCenter, m_PantsCenterMesh},
                {SkinPart.PantsLeft01, m_PantsLeft01Mesh},
                {SkinPart.PantsLeft02, m_PantsLeft02Mesh},
                {SkinPart.PantsRight01, m_PantsRight01Mesh},
                {SkinPart.PantsRight02, m_PantsRight02Mesh},
            };

            m_IsInitialized = true;
        }

        public void PlayFootsteps()
        {
            AudioManager.Instance.PlayFootsteps(transform.position);
        }

        public void PlayDig()
        {
            AudioManager.Instance.PlayDig(transform.position);
        }

        public void PlayDrinkPotion()
        {
            AudioManager.Instance.PlayDrinkPotion();
        }

        public void CreateRifleMuzzleFlash()
        {
            Timer.Instance.Wait(0.15f, () => Instantiate(m_RifleMuzzleFlash, m_GunBarrel).DestroyAsVisualEffect());
        }

        public void CreateShotgunMuzzleFlash()
        {
            Timer.Instance.Wait(0.15f, () => Instantiate(m_ShotgunMuzzleFlash, m_GunBarrel).DestroyAsVisualEffect());
        }

        public void CreatePistolMuzzleFlash()
        {
            Timer.Instance.Wait(0.15f, () => Instantiate(m_PistolMuzzleFlash, m_GunBarrel).DestroyAsVisualEffect());
        }

        public void CreateBlockParticles()
        {
            Timer.Instance.Wait(0.15f, () =>
                Instantiate(m_BlockParticles, m_ShieldAttachmentPoint.transform.position, Quaternion.identity)
                    .DestroyAsVisualEffect());
        }

        public void PlayAnimation(string animation)
        {
            if (m_Animator == null)
            {
                return;
            }

            m_Animator.Play(animation, -1, 0);
        }

        public float GetAnimationLength(string name)
        {
            if (m_Animator == null)
            {
                return 0;
            }

            return m_Animator.GetAnimationClipLength(name);
        }

        public Transform GetAttachmentPoint(AttachmentPoint attachmentPoint)
        {
            if (m_AttachmentPoints[attachmentPoint] != null)
            {
                return m_AttachmentPoints[attachmentPoint];
            }

            Debug.LogWarning($"No attachment point of type {attachmentPoint} on actor {name}");
            return transform;
        }

        private SpriteMeshInstance GetEquipmentMesh(SkinPart part)
        {
            return m_BodyParts.GetValueOrDefault(part, null);
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

                GetEquipmentMesh(skinPart.Part).spriteMesh = m_EmptySpriteMesh;
            }
        }

        public void ShowHelm()
        {
            m_HeadAttachmentPoint.gameObject.SetActive(true);
        }

        public void HideHelm()
        {
            m_HeadAttachmentPoint.gameObject.SetActive(false);
        }

        public void ShowHair()
        {
            m_Hairstyle.gameObject.SetActive(true);
        }

        public void HideHair()
        {
            m_Hairstyle.gameObject.SetActive(false);
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
            hairstyle.transform.SetParent(m_Hairstyle.transform.parent);
            hairstyle.transform.localRotation = Quaternion.identity;
            hairstyle.transform.localPosition = Vector3.zero;
            hairstyle.transform.localScale = Vector3.one;
            hairstyle.gameObject.SetActive(m_Hairstyle.gameObject.activeSelf);

            Destroy(m_Hairstyle.gameObject);

            m_Hairstyle = hairstyle;
        }

        public void ChangeBeard(SpriteGroup beard)
        {
            beard.gameObject.layer = gameObject.layer;
            beard.transform.SetParent(m_Beard.transform.parent);
            beard.transform.localRotation = Quaternion.identity;
            beard.transform.localPosition = Vector3.zero;
            beard.transform.localScale = Vector3.one;
            beard.gameObject.SetActive(m_Beard.gameObject.activeSelf);

            Destroy(m_Beard.gameObject);

            m_Beard = beard;
        }

        public void ChangeScale(float scale)
        {
            transform.localScale *= scale;
        }

        public void ResetScale()
        {
            transform.localScale = m_OriginalScale;
        }

        public void ChangeColor(Color color)
        {
            ResetColor();

            foreach (var mesh in GetComponentsInChildren<SpriteMeshInstance>())
            {
                m_OriginalColors.Add(mesh, mesh.color);
                mesh.color = color;
            }
        }

        public void ResetColor()
        {
            foreach (var pair in m_OriginalColors)
            {
                if (pair.Key == null)
                {
                    continue;
                }

                pair.Key.color = pair.Value;
            }

            m_OriginalColors.Clear();
        }

        public void ChangeHairColor(Color color)
        {
            m_Hairstyle.ChangeColor(color);
        }

        public void ChangeBeardColor(Color color)
        {
            m_Beard.ChangeColor(color);
        }

        public void ChangeSkinColor(Color color)
        {
            m_Skin.ChangeColor(color);
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