using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [Serializable]
    public class SpriteName {
        public AbilityName name;
        public Sprite sprite;
    }
    [SerializeField] private Sprite CooldownSprite;
    [SerializeField] private float cooldownOpacity;
    [SerializeField] private SpriteName[] snArr;


    private Dictionary<AbilityName, Sprite> sprites = new Dictionary<AbilityName, Sprite>();

    void Start(){
        clear();
        foreach (var sn in snArr){
            if (sprites.ContainsKey(sn.name))continue;
            sprites.Add(sn.name, sn.sprite);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("j")) Add(AbilityName.GuardianAngle, 3);
        if (Input.GetKeyDown("k")) clear();
    }
    private (GameObject,Coroutine) Draw(Sprite s, float cooldownDurationInSec){
        //create image with sprite as child
        Image newImg = new GameObject().AddComponent<Image>();
        newImg.transform.SetParent(this.transform);
        newImg.rectTransform.localScale = Vector2.one;
        newImg.sprite = s;

        //adjust size
        float ratio = (float)s.texture.height / s.texture.width;
        float width = GetComponent<RectTransform>().sizeDelta.x;
        newImg.rectTransform.sizeDelta = new Vector2(width, ratio*width);

        Coroutine correspondingCooldownCoRo = null;
        if(cooldownDurationInSec > 0)
            correspondingCooldownCoRo = DrawCooldownOverlay(newImg, cooldownDurationInSec);
        
        return (newImg.gameObject,correspondingCooldownCoRo);
    }
    
    public (GameObject,Coroutine) Add(AbilityName an){
        return Add(an, -1);//Add ability w//o cooldown
    }
    public (GameObject,Coroutine) Add(AbilityName an, float cooldownDurationInSec){
        if (!sprites.ContainsKey(an)) {
            Debug.Log("Add error");
            return (null,null);
        }
        return Draw(sprites[an], cooldownDurationInSec);
    }

    public void clear(){
        StopAllCoroutines();
        foreach (Transform child in transform){
            Destroy(child.GameObject());
        }
    }

    public void stopCooldown(GameObject g,Coroutine c){
        StopCoroutine(c);
        Destroy(g);
    }

    private Coroutine DrawCooldownOverlay(Image ability, float cooldownDurationInSec){
        //create cooldown overlay image as child
        Image cooldown = new GameObject().AddComponent<Image>();
        cooldown.transform.SetParent(ability.transform);
        cooldown.rectTransform.localScale = Vector2.one;
        cooldown.rectTransform.sizeDelta = ability.rectTransform.sizeDelta;
        cooldown.sprite = CooldownSprite;
        cooldown.color = new Color(cooldown.color.r, cooldown.color.g, cooldown.color.b, cooldownOpacity);

        //cooldown.color = Color.red;
        //start clock animation
        cooldown.type = Image.Type.Filled;
        cooldown.fillOrigin = (int)Image.Origin360.Top;
        cooldown.fillAmount = 0;
        return StartCoroutine(cooldownIEnum(cooldown ,cooldownDurationInSec));
    }
    private IEnumerator cooldownIEnum(Image img, float cooldownDurationInSec){
        while (img.fillAmount < 1) {
            img.fillAmount += 1f / cooldownDurationInSec * Time.deltaTime;
            yield return null;
        }
        Destroy(img.gameObject.transform.parent.gameObject);
    }

    public enum AbilityName{
        RepairKit, GuardianAngle, Shield, PiercingShots, SearchingProjectiles, DoubleShot, XPMultiplier, Sabotage, Overcharge
    }
}
