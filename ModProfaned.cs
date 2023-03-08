using System;
using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Effects;
using System.Linq;

[Serializable]
public class ModProfaned : IModification
{
	private const string ACTIVATION_LIQUID = "blood";
	public ModProfaned() { }
	public ModProfaned(int Tier) : base(Tier) { }

	public override bool AllowStaticRegistration()
	{
		return true;
	}
	public override void Register(GameObject Object)
	{
		Object.RegisterPartEvent(this, "LauncherProjectileHit");
		Object.RegisterPartEvent(this, "WeaponDealDamage");
		Object.RegisterPartEvent(this, "WeaponPseudoThrowHit");
		Object.RegisterPartEvent(this, "WeaponThrowHit");
		base.Register(Object);
	}

	public override void ApplyModification(GameObject obj)
    {
		IncreaseDifficulty(1);
	}
    public override bool ModificationApplicable(GameObject Object)
	{
		if (Object.HasPart("MeleeWeapon"))
			return true;
		if (Object.HasPart("MissileWeapon"))
			return true;
		if (Object.HasPart("ThrownWeapon"))
			return true;

		return false;
	}

	public override bool WantEvent(int ID, int cascade)
	{
		return
			base.WantEvent(ID, cascade)
			|| ID == GetDisplayNameEvent.ID
			|| ID == GetShortDescriptionEvent.ID
        ;
	}
	public override bool HandleEvent(GetDisplayNameEvent E)
	{
		if(!E.Object.HasProperName)
        {
			if (E.Understood())
			{
				if (ActivationCriteria(ParentObject))
					E.AddAdjective("{{r-r-R-r-r-r-r-r sequence|profaned}}");
				else
					E.AddAdjective("{{K-K-y-Y-Y-Y-y-K sequence|profaned}}");
			}
			else // if not understood
			{
				E.AddAdjective("{{|&Yenigmatic}}");
			}
		}

		
		return base.HandleEvent(E);
	}
    public override bool HandleEvent(GetShortDescriptionEvent E)
    {
		E.Postfix.AppendRules(GetDescription(Tier, E.Object));
		return base.HandleEvent(E);
	}


    public override bool FireEvent(Event E)
	{
		XRL.Messages.MessageQueue.AddPlayerMessage("Event fired: " + E.ID);

		if(ActivationCriteria(ParentObject))
        {
			bool damageDealt = (E.ID == "WeaponDealDamage" || E.ID == "LauncherProjectileHit" || E.ID == "WeaponThrowHit" || E.ID == "WeaponPseudoThrowHit");
			if (damageDealt && E.GetParameter("Damage") is Damage damage && damage.Amount > 0)
			{
				damage.Amount += Tier * 100;
			}
		}
		
		return base.FireEvent(E);
	}
	public static string GetDescription(int Tier, GameObject obj)
	{
		if(ActivationCriteria(obj))
            return $"Profaned (active): Because this weapon is {ACTIVATION_LIQUID}-stained, it deals an additional {Tier} damage on hit, as long as it penetrates at least once.";
        else
			return $"Profaned ({{{{|&Kinactive}}}}): When {ACTIVATION_LIQUID}-stained, this weapon will deal an additional {Tier} damage on hit, as long as it penetrates at least once.";
	}
	private static bool ActivationCriteria(GameObject obj)
	{
        // get LiquidStained and LiquidCovered effects
        var liquidStained = obj.GetEffect<LiquidStained>();
        var liquidCovered = obj.GetEffect<LiquidCovered>();

        // if either contains blood, return true
        if (liquidStained?.Liquid?.ContainsLiquid(ACTIVATION_LIQUID) == true)
            return true;
		if (liquidCovered?.Liquid?.ContainsLiquid(ACTIVATION_LIQUID) == true)
			return true;

		return false;
	}
}
