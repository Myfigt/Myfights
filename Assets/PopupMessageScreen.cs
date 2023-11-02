using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PopupMessageScreen : UIScreen
{
    [SerializeField]
    TMP_Text MessageBox;
    [SerializeField]
    TMP_Text YesButtonText;
    [SerializeField]
    TMP_Text NoButtonText;
    [SerializeField]
    Button OKButton;
    [SerializeField]
    Button CancelButton;
    WSSocketMessageBase messagedata;
    // Start is called before the first frame update
    void Start()
    {
        YesButtonText.text = "Yes";
        NoButtonText.text = "No";
    }
    WS_ActionType myAction;
    string payLoadData;
    public override void Initialize(params object[] _params)// 0 payload data //1 action type.
    {
        payLoadData = _params[0] as string;

        if (_params.Count() == 2)
        {
            myAction = (WS_ActionType)_params[1];

            switch (myAction)
            {
                case WS_ActionType.make_user_online_offline:
                    break;
                case WS_ActionType.add_friend:
                    MessageBox.text = "You have a new friend Request";
                    break;
                case WS_ActionType.join_room:
                    messagedata = Newtonsoft.Json.JsonConvert.DeserializeObject<GameChallangeMessage>(_params[0] as string);
                   // var test = UIController.Instance._myprofile._allFriends.Find(x => x.Friend_id == (messagedata as GameChallangeMessage).id);
                    MessageBox.text = $"you Friend {UIController.Instance._myprofile._allFriends.Find(x => x.Friend_id == (messagedata as GameChallangeMessage).user_id).Friends_name} is inviting you for a fight";
                    YesButtonText.text = "Fight";
                    NoButtonText.text = "Decline";

                    break;
                case WS_ActionType.accept_friend_request:
                    MessageBox.text = "you are now friends with";
                    break;
                default:
                    break;
            }
        }
        else
        {
            MessageBox.text = _params[0] as string;
        }
        StartCoroutine(Hide());
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(5);
        gameObject.SetActive(false);
        myAction = WS_ActionType.None;
    }


    public void OKButtonPressed()
    {
        //perform action 
        switch (myAction)
        {
            case WS_ActionType.None:
                break;
            case WS_ActionType.make_user_online_offline:
                break;
            case WS_ActionType.add_friend:
                break;
            case WS_ActionType.join_room:
                UIController.Instance.OnFriendChallangeAccepted((messagedata as GameChallangeMessage).room_id);
                break;
            case WS_ActionType.accept_friend_request:
                break;
            default:
                break;
        }
        gameObject.SetActive(false);
    }
    public void CancelButtonPressed()
    {
        gameObject.SetActive(false);
    }

}
