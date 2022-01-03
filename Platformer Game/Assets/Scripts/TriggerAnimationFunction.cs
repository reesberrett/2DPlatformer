/*Copyright Jeremy Blair 2021
License (Creative Commons Zero, CC0)
http://creativecommons.org/publicdomain/zero/1.0/

You may use these scripts in personal and commercial projects.
Credit would be nice but is not mandatory.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimationFunction : MonoBehaviour
{
    public GameObject ObjectToAnimate;
    public string AnimationParameterName = "";
    public string AnimationParameterValue = "";
       

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddPoints()
    {

    }

 

    private void SetParameter(Animator animatorOnObject,string paramName, string paramValue)
    {
        foreach (AnimatorControllerParameter param in animatorOnObject.parameters)
        {
            if (param.name.ToLower() == paramName.ToLower())
            {
                AnimatorControllerParameterType paramType = param.type;
                if (paramType == AnimatorControllerParameterType.Bool)
                {
                    anim.SetBool(param.name, bool.Parse(paramValue));
                }
                else if (paramType == AnimatorControllerParameterType.Float)
                {
                    anim.SetFloat(param.name, float.Parse(paramValue));
                }
                else if (paramType == AnimatorControllerParameterType.Int)
                {
                    anim.SetInteger(param.name, int.Parse(paramValue));
                }
                else if (paramType == AnimatorControllerParameterType.Trigger)
                {
                    animatorOnObject.SetTrigger(param.name);
                }
            }
        }
    }

    private void DoAnimation()
    {
        if (!enabled)
            return;
        Animator animatorOnObject = anim;
        SetParameter(anim, AnimationParameterName, AnimationParameterValue);
    }
}
