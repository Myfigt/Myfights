using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FighterListManager : UIScreen
{
    [SerializeField]
    GameObject FighterTemplateObject;
    [SerializeField]
    Transform _content;
    List<Fighter> MyFighters = null;

    public Fighter SelectedFighter;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Initialize(List<Fighter> _allFighters)
    {
        for (int i = 0; i < _content.childCount; i++)
        {
            if (_content.GetChild(i).gameObject.activeSelf)
            {
                DestroyImmediate(_content.GetChild(i).gameObject);
                i--;
            }
        }
        for (int i = 0; i < _allFighters.Count; i++)
        {
            GameObject fighter = GameObject.Instantiate(FighterTemplateObject, _content);
            fighter.GetComponentInChildren<TMPro.TMP_Text>().text = _allFighters[i].Name;
            StartCoroutine(GetProfileImage(_allFighters[i].Photo, fighter.transform.GetChild(0).GetComponent<Image>()));
            //fighter.GetComponent<Button>().onClick.AddListener(() => OnFighterSelected(i));
            fighter.SetActive(true);
        }
       
        MyFighters = _allFighters;
    }
    // Update is called once per frame
    public void OnFighterSelectionChanged(Vector2 vector)
    {

    }
    public void OnFighterSelected(TMPro.TMP_Text index)
    {
        SelectedFighter = MyFighters.Find(item => item.Name == index.text);
        UIController.Instance.SetupScreen(UIController.Screens.Figher_VideoSelection);
        UIController.Instance._VideoSelection.Initialize(SelectedFighter);
    }

    IEnumerator GetProfileImage(string MediaUrl ,Image _image)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {

            Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            _image.overrideSprite = sprite;
        }
    }
}
