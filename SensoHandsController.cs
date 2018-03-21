using UnityEngine;

public class SensoHandsController : SensoBaseController
{
    // Variables for hands objects
    [Header("Glove tracking")]
    public Senso.Hand[] Hands;
    private int m_rightHandInd = -1;
    private int m_leftHandInd = -1;

    // Initialization
    void Start () {
        if (Hands != null && Hands.Length > 0) {
            for (int i = 0; i < Hands.Length; ++i)
            {
                if (m_rightHandInd == -1 && Hands[i].HandType == Senso.EPositionType.RightHand)
                {
                    m_rightHandInd = i;
                    Hands[i].SetController(this);
                }
                else if (m_leftHandInd == -1 && Hands[i].HandType == Senso.EPositionType.LeftHand)
                {
                    m_leftHandInd = i;
                    Hands[i].SetController(this);
                }
            }
        }
        base.Start();
    }

    // Every frame
    void Update ()
    {
        base.Update();
        if (sensoThread != null)
        {
            int cnt = sensoThread.UpdateData(ref receivedData);
            if (cnt > 0)
            {
                bool rightUpdated = false;
                bool leftUpdated = false;
                for (int i = cnt - 1; i >= 0; --i)
                {
                    var parsedData = receivedData[i];
                    if (parsedData.type.Equals("position"))
                    {
                        if ((m_rightHandInd != -1 && !rightUpdated) || (m_leftHandInd != -1 && !leftUpdated))
                        {
                            var handData = JsonUtility.FromJson<Senso.HandDataFull>(parsedData.packet);
                            if (handData.data.handType == Senso.EPositionType.RightHand && m_rightHandInd != -1 && !rightUpdated)
                            {
                                setHandPose(ref handData, m_rightHandInd);
                                rightUpdated = true;
                            }
                            else if (handData.data.handType == Senso.EPositionType.LeftHand && m_leftHandInd != -1 && !leftUpdated)
                            {
                                setHandPose(ref handData, m_leftHandInd);
                                leftUpdated = true;
                            }
                        }
                        else break;
                    }
                }
            }
        }
	}

    public void SendVibro(Senso.EPositionType handType, Senso.EFingerType finger, ushort duration, byte strength)
    {
        sensoThread.VibrateFinger(handType, finger, duration, strength);
    }

    private void setHandPose(ref Senso.HandDataFull handData, int ind)
    {
        if (Hands[ind].MacAddress == null)
        {
            Hands[ind].SetMacAddress(handData.src);
        }
        Hands[ind].SetSensoPose(handData.data);
    }
}
