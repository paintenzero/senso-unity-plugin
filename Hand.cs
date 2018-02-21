using UnityEngine;

namespace Senso
{
    public abstract class Hand : MonoBehaviour
    {
        public EPositionType HandType;
        protected System.WeakReference m_controller;
        public int BatteryLevel { get; private set; }
        public int Temperature { get; private set; }
        public string MacAddress { get; private set; }
		public HandData Pose { get; private set; }

        public void Start()
        {
            MacAddress = null;
        }

        public void SetBattery(int newLevel)
        {
            BatteryLevel = newLevel;
        }
        public void SetTemperature(int newTemp)
        {
            Temperature = newTemp;
        }

        public void SetMacAddress(string newAddress)
        {
            MacAddress = newAddress;
        }

        public void SetController(SensoHandsController aController)
        {
            m_controller = new System.WeakReference(aController);
        }

        public void VibrateFinger(Senso.EFingerType finger, ushort duration, byte strength)
        {
            if (m_controller.IsAlive)
            {
                SensoHandsController man = m_controller.Target as SensoHandsController;
                if (man != null)
                    man.SendVibro(HandType, finger, duration, strength);
            }
        }

        public virtual void SetSensoPose (HandData newData) 
		{
			Pose = newData;
		}
    }
}
