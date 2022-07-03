using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TintInteractable : MonoBehaviour
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct ShaderPropertyLookup
	{
		public static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
	}

	[Tooltip("Tint color for interactable.")]
	[SerializeField]
	Color m_TintColor = Color.yellow;
	[SerializeField]
	[Tooltip("Renderer(s) to use for tinting (will default to any Renderer on the GameObject if not specified).")]
	private List<Renderer> m_TintRenderers = new List<Renderer>();

	bool tinted = false;
	float lastTintMagnitude = 0;
	bool shouldTint = false;
	float tintMagnitude = 0;
	float tintModify = 1.0f;
	private MaterialPropertyBlock m_TintPropertyBlock;

	private bool m_EmissionEnabled;

	private bool m_HasLoggedMaterialInstance;

	private static readonly List<Material> s_Materials = new List<Material>();

	public Color tintColor
	{
		get
		{
			return m_TintColor;
		}
		set
		{
			m_TintColor = value;
		}
	}
	
	public List<Renderer> tintRenderers
	{
		get
		{
			return m_TintRenderers;
		}
		set
		{
			m_TintRenderers = value;
		}
	}

	protected void Awake()
	{
		if (m_TintRenderers.Count == 0)
		{
			GetComponents(m_TintRenderers);
			if (m_TintRenderers.Count == 0)
			{
				Debug.LogWarning($"Could not find required Renderer component on {base.gameObject} for tint visual.", this);
			}
		}

		m_EmissionEnabled = GetEmissionEnabled();
		m_TintPropertyBlock = new MaterialPropertyBlock();
	}

	protected void OnDestroy()
	{
	}

	private void LateUpdate()
	{
		SetTint(shouldTint);
		shouldTint = false;
		tintModify = 1.0f;
	}

	public void RequestTintModifier(float modify)
	{
		tintModify = modify;
	}
	public void RequestTint( float tintMagnitude)
	{
		if (!shouldTint)
			this.tintMagnitude = tintMagnitude;
		else
			this.tintMagnitude = Mathf.Max(this.tintMagnitude, tintMagnitude);
		
		shouldTint = true;
	}

	public void SetTint(bool on)
	{
		if (tinted == on && tintMagnitude == lastTintMagnitude)
			return;
		tinted = on;
		lastTintMagnitude = tintMagnitude;

		Color value = on ? (m_TintColor * Mathf.LinearToGammaSpace(1f)) : Color.black;
		value *= tintMagnitude * tintModify;

		if (!m_EmissionEnabled && !m_HasLoggedMaterialInstance)
		{
			Debug.LogWarning("Emission is not enabled on a Material used by a tint visual, a Material instance will need to be created.", this);
			m_HasLoggedMaterialInstance = true;
		}

		foreach (Renderer tintRenderer in m_TintRenderers)
		{
			if (tintRenderer == null)
			{
				continue;
			}

			if (!m_EmissionEnabled)
			{
				tintRenderer.GetMaterials(s_Materials);
				foreach (Material s_Material in s_Materials)
				{
					if (on)
					{
						s_Material.EnableKeyword("_EMISSION");
					}
					else
					{
						s_Material.DisableKeyword("_EMISSION");
					}
				}

				s_Materials.Clear();
			}

			tintRenderer.GetPropertyBlock(m_TintPropertyBlock);
			m_TintPropertyBlock.SetColor(ShaderPropertyLookup.emissionColor, value);
			tintRenderer.SetPropertyBlock(m_TintPropertyBlock);
		}
	}

	protected virtual bool GetEmissionEnabled()
	{
		foreach (Renderer tintRenderer in m_TintRenderers)
		{
			if (tintRenderer == null)
			{
				continue;
			}

			tintRenderer.GetSharedMaterials(s_Materials);
			foreach (Material s_Material in s_Materials)
			{
				if (!s_Material.IsKeywordEnabled("_EMISSION"))
				{
					s_Materials.Clear();
					return false;
				}
			}
		}

		s_Materials.Clear();
		return true;
	}

}
