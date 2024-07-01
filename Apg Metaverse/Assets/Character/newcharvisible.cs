/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newcharvisible : MonoBehaviour
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
 
    public class canMouseLook : MonoBehaviour
    {

        Vector2 mouseLook;
        Vector2 smoothV;

        public float sensitivity = 5.0f;
        public float smoothing = 2.0f;

        GameObject character;

        void Start()
        {

            character = this.transform.parent.gameObject;

        }

        void Update()
        {

            //md = mass delta

            var md = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxisRaw("Mouse Y"));


            md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

            smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);

            smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);

            mouseLook += smoothV;

            mouseLook.y = Mathf.Clamp(mouseLook.y, 40, -50)

            character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);



        }
    }
}
*/