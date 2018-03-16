using UnityEngine;

public class SensoBodyController : SensoBaseController {

    // Variables for hands objects
    [Header("Body tracking")]
    public Senso.Body Avatar;

    public TextMesh statsMesh;

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
        var t = Time.realtimeSinceStartup;
        base.Update();
        var sendDt = Time.realtimeSinceStartup - t;
        if (sensoThread != null)
        {
            var t2 = Time.realtimeSinceStartup;
            int cnt = sensoThread.UpdateData(ref receivedData);
            var updateDt = Time.realtimeSinceStartup - t2;
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
            var dt = Time.realtimeSinceStartup - t;
            if (dt > 0.5 || maxFrameUpdate == 0)
            {
                maxFrameUpdate = dt;
                statsMesh.text = string.Format("b:{0:0.00},c:{1},s:{2:0.00},u:{3:0.00}", dt, cnt, sendDt, updateDt);
            }
        }
	}

}
