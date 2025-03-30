namespace CaptainCoder.Dungeoneering.Encounter
{
    public interface ILivingEntityRenderer
    {
        public void Render(LivingEntityData data);
    }

    public interface IHeroEntityRenderer : ILivingEntityRenderer
    {
        public void Render(HeroEntityData data);
    }
}