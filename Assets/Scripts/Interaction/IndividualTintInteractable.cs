using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualTintInteractable : TintInteractable
{
	[SerializeField]
	int tintIndex = 0;

	Material tintMaterial;

	protected override void Awake()
	{
		List<Material> materials = new List<Material>();
		tintRenderers[0].GetMaterials(materials);
		if (tintIndex >= materials.Count)
			tintIndex = 0;

		tintMaterial = materials[tintIndex];
		m_EmissionEnabled = GetEmissionEnabled();
	}

	public override void SetTint(bool on)
	{
		if (tinted == on && (!on || tintMagnitude * tintModify == lastTintMagnitude))
			return;
		tinted = on;
		lastTintMagnitude = tintMagnitude * tintModify;

		Color value = on ? (m_TintColor * Mathf.LinearToGammaSpace(1f)) : Color.black;
		value *= tintMagnitude * tintModify;

		if (!m_EmissionEnabled && !m_HasLoggedMaterialInstance)
		{
			Debug.LogWarning("Emission is not enabled on a Material used by a tint visual, a Material instance will need to be created.", this);
			m_HasLoggedMaterialInstance = true;
		}

		if (!m_EmissionEnabled)
		{
			if (on)
			{
				tintMaterial.EnableKeyword("_EMISSION");
			}
			else
			{
				tintMaterial.DisableKeyword("_EMISSION");
			}
		}

		tintMaterial.SetColor(ShaderPropertyLookup.emissionColor, value);
	}

	protected override bool GetEmissionEnabled()
	{
		return tintMaterial.IsKeywordEnabled("_EMISSION");
	}
}
