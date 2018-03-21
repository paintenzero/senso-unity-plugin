using UnityEngine;

public class SensoBodyController : SensoBaseController {

    // Variables for hands objects
    [Header("Body tracking")]
    public Senso.Body Avatar;

    // Initialization
    void Start ()
    {
        if (Avatar == null)
        {
            Debug.LogError("Avatar is not set! What am I supposed to control?");
            return;
        }
        Avatar.SetController(this);

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
                bool positionSet = false;
                for (int i = cnt - 1; i >= 0; --i)
                {
                    var parsedData = receivedData[i];
                    if (parsedData.type.Equals("avatar") || parsedData.type.Equals("campos"))
                    {
                        if (!positionSet)
                        {
                            var bodyData = JsonUtility.FromJson<Senso.BodyDataFull>(parsedData.packet);
                            if (bodyData != null)
                            {
                                Avatar.SetSensoPose(bodyData.data);
                                positionSet = true;
                            }
                        }
                        else break;
                    }
                }
            }
        }
	}

}
