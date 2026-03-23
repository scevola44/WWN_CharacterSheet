using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;
using WWN.Domain.ValueObjects;

namespace WWN.Application.Services;

/// <summary>
/// Seeds the FocusDefinitions table with the standard Foci from the
/// Worlds Without Number Free Edition rulebook (Sine Nomine Publishing).
/// </summary>
public class FocusDefinitionSeeder(IFocusDefinitionRepository focusDefinitionRepository)
{
    public async Task SeedIfEmptyAsync(CancellationToken ct = default)
    {
        if (await focusDefinitionRepository.AnyAsync(ct)) return;

        foreach (var focusDefinition in CreateDefaultFoci())
            await focusDefinitionRepository.AddAsync(focusDefinition, ct);
    }

    private static IEnumerable<FocusDefinition> CreateDefaultFoci()
    {
#region Combat Foci
        yield return new FocusDefinition(
            name: "Alert",
            level1Description:
                "Gain Notice as a bonus skill. You cannot be surprised, and enemies cannot use " +
                "Execution Attacks against you. When your group rolls initiative you may roll " +
                "twice and take the better result.",
            level2Description:
                "You always act first in a combat round unless another combatant is also Alert. " +
                "You may choose to act at any point in the round.",
            description:
                "You are keenly aware of your surroundings and virtually impossible to take unaware.",
            level1Effects: new[]
            {
                new FocusEffect(FocusEffectType.SkillBonus, 1, TargetSkill: SkillName.Notice),
            });

        yield return new FocusDefinition(
            name: "Armsmaster",
            level1Description:
                "Gain Stab as a bonus skill. You have a +1 bonus to hit with melee and thrown " +
                "weapons. You may add your Stab skill level to damage with melee and thrown " +
                "weapons, in addition to any attribute bonus.",
            level2Description:
                "Your melee and thrown weapon hit bonus increases to +2. The Shock damage of " +
                "your melee and thrown weapons improves by +2 and applies regardless of the " +
                "target's armor class.",
            description:
                "You have unusual competence with thrown weapons and melee attacks. This focus " +
                "does not apply to unarmed attacks or non-thrown projectile weapons, and its " +
                "bonuses do not stack with Deadeye or similar foci.",
            level1Effects: new[]
            {
                new FocusEffect(FocusEffectType.SkillBonus, 1, TargetSkill: SkillName.Stab),
                new FocusEffect(FocusEffectType.AttackBonus, 1,
                    Condition: FocusEffectCondition.StabWeapon),
                new FocusEffect(FocusEffectType.DamageBonus, 0,
                    FocusEffectValueType.SkillLevel, FocusEffectCondition.StabWeapon,
                    TargetSkill: SkillName.Stab),
            },
            level2Effects: new[]
            {
                new FocusEffect(FocusEffectType.AttackBonus, 1,
                    Condition: FocusEffectCondition.StabWeapon),
                new FocusEffect(FocusEffectType.ShockBonus, 2,
                    Condition: FocusEffectCondition.StabWeapon),
            });

        yield return new FocusDefinition(
            name: "Assassin",
            level1Description:
                "Gain Sneak as a bonus skill. You can perform Execution Attacks with any weapon " +
                "that deals damage. You may make a Sneak or Surprise attack even after combat " +
                "has started, provided you were unseen at the beginning of that attack.",
            level2Description:
                "You can perform Execution Attacks on targets that are aware of you, provided " +
                "they are engaged with other enemies and have not spotted you yet this round.",
            description:
                "You are exceptionally skilled at swiftly killing targets before they can react.");

        yield return new FocusDefinition(
            name: "Close Combatant",
            level1Description:
                "Gain Punch or Stab as a bonus skill. You suffer no attack penalty for fighting " +
                "in cramped quarters, underwater, or while grappling. You suffer no off-hand " +
                "penalty when fighting with a weapon in each hand.",
            level2Description:
                "Once per round, as a free action after your normal Main Action attack, you may " +
                "make an additional attack with an off-hand weapon or unarmed strike.",
            description:
                "You are trained for fighting in tight quarters, excelling at close-range melee " +
                "where most fighters struggle.");

        yield return new FocusDefinition(
            name: "Deadeye",
            level1Description:
                "Gain Shoot as a bonus skill. You may add your Shoot skill level to the damage " +
                "roll of ranged attacks, in addition to any attribute bonus.",
            level2Description:
                "You have a +1 bonus to hit with all ranged attacks. Your ranged attacks ignore " +
                "any AC bonus that targets receive from cover.",
            description:
                "You are an exceptional marksman, consistently finding weak spots in defenses " +
                "from a distance.",
            level1Effects: new[]
            {
                new FocusEffect(FocusEffectType.SkillBonus, 1, TargetSkill: SkillName.Shoot),
                new FocusEffect(FocusEffectType.DamageBonus, 0,
                    FocusEffectValueType.SkillLevel, FocusEffectCondition.ShootWeapon,
                    TargetSkill: SkillName.Shoot),
            },
            level2Effects: new[]
            {
                new FocusEffect(FocusEffectType.AttackBonus, 1,
                    Condition: FocusEffectCondition.ShootWeapon),
            });

        yield return new FocusDefinition(
            name: "Die Hard",
            level1Description:
                "Gain Survive as a bonus skill. You gain +2 maximum hit points per character " +
                "level. When you are mortally wounded you automatically stabilize and do not " +
                "need to make a Physical saving throw to avoid death.",
            level2Description:
                "Once per scene, when you are reduced to 0 hit points, you may roll 1d6. On a " +
                "result of 4 or higher, you remain conscious at 1 hit point instead.",
            description:
                "You are exceptionally difficult to kill, hardened by experience and sheer " +
                "physical resilience.",
            level1Effects: new[]
            {
                new FocusEffect(FocusEffectType.SkillBonus, 1, TargetSkill: SkillName.Survive),
                new FocusEffect(FocusEffectType.HpBonus, 2, FocusEffectValueType.Level),
            });

        yield return new FocusDefinition(
            name: "Impervious Defense",
            level1Description:
                "Gain a +2 bonus to your Armor Class at all times, regardless of armor worn.",
            level2Description:
                "Your AC bonus increases to +3. Additionally, your armor class cannot be " +
                "reduced by enemy special attacks, abilities, or situational penalties.",
            description:
                "You have an uncanny ability to avoid blows through anticipation, footwork, " +
                "and subtle deflection.",
            level1Effects: new[]
            {
                new FocusEffect(FocusEffectType.AcBonus, 2),
            },
            level2Effects: new[]
            {
                new FocusEffect(FocusEffectType.AcBonus, 1),
            });

        yield return new FocusDefinition(
            name: "Ironhide",
            level1Description:
                "Gain Exert as a bonus skill. Your skin is tough as leather through conditioning " +
                "and natural resilience. Your base Armor Class without any worn armor is 13.",
            level2Description:
                "Your natural base AC without armor increases to 15. You gain a +1 bonus to all " +
                "Physical saving throws.",
            description:
                "Your body has been toughened through years of conditioning, giving you a " +
                "naturally resilient hide.");

        yield return new FocusDefinition(
            name: "Savage Fray",
            level1Description:
                "Gain Stab or Punch as a bonus skill. When you kill or incapacitate an enemy " +
                "with a melee attack, you may immediately make a free melee attack against " +
                "another adjacent enemy as a free action.",
            level2Description:
                "After killing or incapacitating a foe, you may continue making free attacks " +
                "against additional adjacent enemies until you miss or there are no more " +
                "adjacent enemies to attack.",
            description:
                "You fight with savage ferocity, turning the momentum of each kill into an " +
                "opportunity to strike another foe.");

        yield return new FocusDefinition(
            name: "Shocking Assault",
            level1Description:
                "Gain Stab or Punch as a bonus skill. The Shock damage from your melee and " +
                "unarmed attacks applies to all targets regardless of their Armor Class.",
            level2Description:
                "The Shock damage of your melee and unarmed attacks increases by +2.",
            description:
                "Your attacks carry tremendous force and always deal at least some harm, " +
                "even when partially deflected.");

        yield return new FocusDefinition(
            name: "Skirmisher",
            level1Description:
                "Gain Exert as a bonus skill. You can move your full movement and still make " +
                "a ranged attack without penalty. You do not provoke free attacks when " +
                "retreating from melee combat.",
            level2Description:
                "Once per round, after making a ranged attack, you may move up to half your " +
                "movement speed as a free action.",
            description:
                "You are highly mobile in combat, fighting most effectively while constantly " +
                "in motion.");

        yield return new FocusDefinition(
            name: "Slayer",
            level1Description:
                "Gain any combat skill as a bonus skill. You deal +2 damage to targets that " +
                "are currently below half their maximum hit points.",
            level2Description:
                "When you kill an enemy in melee, nearby allies within 30 feet gain a +1 bonus " +
                "to their next attack roll. Enemies you kill cannot be reanimated as undead by " +
                "standard magical means.",
            description:
                "You have a talent for delivering final, decisive blows and dispatching " +
                "weakened foes efficiently.");

        yield return new FocusDefinition(
            name: "Sniper",
            level1Description:
                "Gain Shoot as a bonus skill. You have a +1 bonus to hit with ranged attacks " +
                "made from a hidden or concealed position. You can make ranged attacks from " +
                "cover without automatically revealing your location.",
            level2Description:
                "When you spend your Main Action aiming before attacking, your next ranged " +
                "attack this round deals maximum possible damage if it hits.",
            description:
                "You are skilled at eliminating targets from concealed positions at long range.");

        yield return new FocusDefinition(
            name: "Unarmed Combatant",
            level1Description:
                "Gain Punch as a bonus skill. Your unarmed attacks deal 1d6 damage plus your " +
                "Punch skill level and your relevant attribute modifier. The Shock damage from " +
                "your unarmed attacks applies regardless of the target's Armor Class.",
            level2Description:
                "Your unarmed attacks deal 1d8 damage plus your Punch skill level and attribute " +
                "modifier. Once per round, when an enemy misses you in melee, you may make a " +
                "free unarmed counterattack against them.",
            description:
                "You have trained your body into a lethal weapon and fight with exceptional " +
                "skill in unarmed combat.");

        yield return new FocusDefinition(
            name: "Valiant Defender",
            level1Description:
                "Gain any combat skill as a bonus skill. When an adjacent ally is attacked, " +
                "you may use an Instant action to interpose yourself, becoming the target of " +
                "that attack instead.",
            level2Description:
                "When you interpose yourself to protect an ally, you may make a free attack " +
                "against the attacker as part of the same Instant action.",
            description:
                "You are dedicated to protecting your allies, willingly placing yourself in " +
                "harm's way to shield others from danger.");

        yield return new FocusDefinition(
            name: "Whirlwind Assault",
            level1Description:
                "Gain Stab or Punch as a bonus skill. Once per round as a free action after " +
                "your normal Main Action attack, you may make a single attack against up to two " +
                "adjacent enemies.",
            level2Description:
                "Your free whirlwind attack can strike up to four adjacent enemies. Each hit " +
                "deals normal weapon damage.",
            description:
                "You fight in a whirlwind of fluid motion, striking multiple opponents with " +
                "devastating speed.");
#endregion Combat Foci

#region Non-Combat Foci
        yield return new FocusDefinition(
            name: "Authority",
            level1Description:
                "Gain Lead as a bonus skill. Ordinary people follow reasonable, non-suicidal " +
                "orders from you without requiring a skill check. Your hirelings and companions " +
                "gain a +1 bonus to their morale saves.",
            level2Description:
                "You may issue orders to NPCs in a crisis. If an NPC fails a morale save while " +
                "you are present, you may spend an Instant action to give them a second save " +
                "at +2.",
            description:
                "You carry a natural air of command that people instinctively respond to.");

        yield return new FocusDefinition(
            name: "Connected",
            level1Description:
                "Gain Connect as a bonus skill. You can always find someone willing to talk, " +
                "trade, or share information in any settled area. Contacts freely share " +
                "non-dangerous information with you.",
            level2Description:
                "You can obtain rare goods and professional services through your network. " +
                "Contacts will assist with moderately risky requests, and even hostile parties " +
                "may grant you an audience due to mutual connections.",
            description:
                "You have an extensive network of contacts, allies, and associates wherever " +
                "you travel.");

        yield return new FocusDefinition(
            name: "Cultured",
            level1Description:
                "Gain Connect or Know as a bonus skill. You know the proper etiquette, customs, " +
                "and social norms of any culture you have studied or spent time with, and never " +
                "give accidental offense through ignorance.",
            level2Description:
                "You can pass convincingly as a member of any culture you have researched, even " +
                "to knowledgeable locals, and can convincingly imitate specific social roles " +
                "such as noble, priest, or wealthy merchant.",
            description:
                "You are well-versed in the customs, arts, and refinements of civilized " +
                "society across many cultures.");

        yield return new FocusDefinition(
            name: "Diplomat",
            level1Description:
                "Gain Talk as a bonus skill. You can attempt to defuse a hostile encounter " +
                "through negotiation before violence breaks out; enemies will pause briefly " +
                "to hear you out.",
            level2Description:
                "Once per scene, you automatically succeed on one social skill check without " +
                "rolling dice. This applies to Talk, Connect, or Lead skill checks.",
            description:
                "You have a gift for negotiation and finding common ground even in the most " +
                "tense situations.");

        yield return new FocusDefinition(
            name: "Gifted Chirurgeon",
            level1Description:
                "Gain Heal as a bonus skill. Patients you treat recover +2 additional hit points " +
                "from your First Aid. You can stabilize a mortally wounded patient as a Move " +
                "action rather than a Main Action.",
            level2Description:
                "You can treat injuries that normally require professional medical facilities " +
                "using only improvised tools and supplies. Stabilization attempts automatically " +
                "succeed and patients regain consciousness after 1 round instead of 10 minutes.",
            description:
                "You have exceptional skill at healing wounds and treating injuries, even " +
                "under the most difficult field conditions.");

        yield return new FocusDefinition(
            name: "Henchkeeper",
            level1Description:
                "Gain Lead as a bonus skill. You may retain up to twice the normal number of " +
                "hirelings and henchmen. All henchmen under your command gain a +1 bonus to " +
                "their morale saves.",
            level2Description:
                "Your henchmen gain an additional +1 to morale saves (total +2 bonus), +1 to " +
                "all attack rolls, and +1 to all skill checks while in your direct service.",
            description:
                "You have a talent for recruiting, retaining, and inspiring loyal hirelings " +
                "and followers.");

        yield return new FocusDefinition(
            name: "Loremaster",
            level1Description:
                "Gain Know as a bonus skill. You may reroll any failed Know skill check once, " +
                "keeping the second result. You can identify the nature and properties of " +
                "magical items and unusual creatures with a Know check.",
            level2Description:
                "You may make Know checks for subjects outside your area of personal expertise. " +
                "Your once-per-check reroll now applies to any failed attribute-based skill " +
                "check, not just Know.",
            description:
                "You have accumulated vast stores of knowledge through tireless research, " +
                "study, and careful observation.");

        yield return new FocusDefinition(
            name: "Nullifier",
            level1Description:
                "Gain Notice as a bonus skill. You gain a +2 bonus to all saving throws against " +
                "magical effects. You can sense the use or presence of active magic within " +
                "30 feet.",
            level2Description:
                "Once per day as an Instant action, you can automatically negate a single " +
                "magical effect that is targeting you, with no saving throw required.",
            description:
                "You have an innate resistance to magical forces and a talent for disrupting " +
                "arcane workings.",
            level1Effects: new[]
            {
                new FocusEffect(FocusEffectType.SkillBonus, 1, TargetSkill: SkillName.Notice),
                new FocusEffect(FocusEffectType.SaveBonus, 2),
            });

        yield return new FocusDefinition(
            name: "Poisoner",
            level1Description:
                "Gain Heal as a bonus skill. You can safely harvest venom from natural creatures " +
                "and prepare toxins from natural ingredients. Targets of your poisons suffer " +
                "a -2 penalty to their saving throws against them.",
            level2Description:
                "You can create specialized poisons that affect beings normally immune to " +
                "natural toxins. Your poisons can be tailored to target specific creature types " +
                "or to produce unusual effects.",
            description:
                "You have expert knowledge of toxins, venoms, and the subtle art of poisoning.");

        yield return new FocusDefinition(
            name: "Rider",
            level1Description:
                "Gain Ride as a bonus skill. You suffer no penalties for combat while mounted. " +
                "You can direct your mount to move, charge, or take evasive action as a free " +
                "action rather than a Move action.",
            level2Description:
                "While mounted, you can perform mounted charges and deal full damage. Your " +
                "skilled guidance grants your mount a +2 bonus to its Armor Class.",
            description:
                "You are an exceptional rider, fighting as effectively from the saddle as " +
                "most warriors do on foot.");

        yield return new FocusDefinition(
            name: "Specialist",
            level1Description:
                "Gain any non-combat skill as a bonus skill. You also gain one extra skill " +
                "point that must be spent on a non-combat skill of your choice.",
            level2Description:
                "You gain one additional extra skill point that must be spent on a non-combat " +
                "skill of your choice.",
            description:
                "You have particular aptitude for developing expertise in non-combat skills. " +
                "This focus can be taken multiple times, each time for a different skill.",
            canTakeMultipleTimes: true);

        yield return new FocusDefinition(
            name: "Spirit Familiar",
            level1Description:
                "Gain Notice as a bonus skill. You attract a small magical animal as a permanent " +
                "companion. You can communicate with it empathically and perceive through its " +
                "senses as an Instant action.",
            level2Description:
                "Your familiar can perform simple magical tasks on your behalf and can carry " +
                "messages or small items. It can serve as a focus for your magic, granting +1 " +
                "to magical skill checks when it is present.",
            description:
                "You have a bond with a small magical creature that serves as your permanent " +
                "companion and assistant.");

        yield return new FocusDefinition(
            name: "Tinker",
            level1Description:
                "Gain Fix as a bonus skill. You can craft and repair mechanical devices and " +
                "equipment without specialized tools. The time required to fix or craft items " +
                "is halved.",
            level2Description:
                "You can craft complex mechanical devices, modify weapons and armor for small " +
                "improvements, and construct unusual mechanisms. Devices you repair gain " +
                "improved reliability.",
            description:
                "You have a remarkable knack for mechanical devices, able to build, repair, " +
                "and improvise with whatever is at hand.");

        yield return new FocusDefinition(
            name: "Trapmaster",
            level1Description:
                "Gain Notice or Sneak as a bonus skill. You can detect and disarm traps and " +
                "simple locks without specialized tools. You can set effective improvised traps " +
                "quickly using available materials.",
            level2Description:
                "Traps you set deal one additional die of damage. You can repurpose and " +
                "redirect existing traps. You can detect magically concealed traps as well as " +
                "purely physical ones.",
            description:
                "You are skilled at creating, detecting, and disarming traps and mechanical " +
                "hazards of all kinds.");

        yield return new FocusDefinition(
            name: "Wanderer",
            level1Description:
                "Gain Survive as a bonus skill. You can always find adequate food, water, and " +
                "shelter in natural environments without a skill check. You never become lost " +
                "in natural terrain.",
            level2Description:
                "You can pass through natural terrain without leaving visible tracks. You can " +
                "navigate underground, at sea, or even in supernatural environments without " +
                "tools or external guides.",
            description:
                "You have an innate connection to the natural world and an exceptional talent " +
                "for surviving and navigating in the wild.");

        yield return new FocusDefinition(
            name: "Wayfinder",
            level1Description:
                "Gain Navigate as a bonus skill. You always know your cardinal direction and " +
                "general location relative to known landmarks. You can reliably determine the " +
                "route to any location you have previously visited.",
            level2Description:
                "You can sense the general direction of locations you know well, even without " +
                "visible landmarks. You can intuit whether a route ahead is safe or recently " +
                "traveled.",
            description:
                "You have an exceptional sense of direction and geography, able to find " +
                "your way anywhere with remarkable ease.");
        
        #endregion Non-Combat Foci
    }
}
