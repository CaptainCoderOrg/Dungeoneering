using System.Collections.Generic;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class HeroSkillsRenderer : MonoBehaviour, IHeroEntityRenderer
    {
        [AssertIsSet][SerializeField] private DieIconController[] _meleeDiceIcons;
        [AssertIsSet][SerializeField] private DieIconController[] _rangeDiceIcons;
        [AssertIsSet][SerializeField] private DieIconController[] _magicDiceIcons;

        public void Render(HeroEntityData data)
        {
            RenderDice(_meleeDiceIcons, data.MeleeSkillDice);
            RenderDice(_rangeDiceIcons, data.RangeSkillDice);
            RenderDice(_magicDiceIcons, data.MagicSkillDice);
        }

        private static void RenderDice(DieIconController[] icons, List<DieData> dice)
        {
            for (int ix = 0; ix < icons.Length; ix++)
            {
                var controller = icons[ix];
                if (dice.Count <= ix) { controller.Hide(); }
                else if (dice[ix] == null) { controller.Hide(); }
                else { controller.Die = dice[ix]; }
            }
        }

        public void Render(LivingEntityData data)
        {
            if (data is HeroEntityData hero)
            {
                Render(hero);
            }
        }
    }
}