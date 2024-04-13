using Rocket.API;

namespace AridBrindasCooldown
{
    public class BrindasVehicleCooldownConfiguration:IRocketPluginConfiguration
    {
        public float DefaultTimeoutInSeconds;
        
        public void LoadDefaults()
        {
            DefaultTimeoutInSeconds = 120;
        }
    }
}