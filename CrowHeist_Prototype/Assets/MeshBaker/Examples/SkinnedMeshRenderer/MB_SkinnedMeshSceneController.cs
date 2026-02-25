using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DigitalOpus.MB.Examples
{
    public class MB_SkinnedMeshSceneController : MonoBehaviour
    {
        public GameObject swordPrefab;
        public GameObject hatPrefab;
        public GameObject glassesPrefab;
        public GameObject workerPrefab;

        public GameObject targetCharacter;

        public MB3_MeshBaker skinnedMeshBaker;

        GameObject swordInstance;
        GameObject glassesInstance;
        GameObject hatInstance;

        void Start()
        {
            //To demonstrate lets add a character to the combined mesh
            GameObject worker1 = (GameObject)Instantiate(workerPrefab);
            worker1.transform.position = new Vector3(1.31f, 0.985f, -0.25f);
            Animation anim = worker1.GetComponent<Animation>();
            anim.wrapMode = WrapMode.Loop;
            //IMPORTANT set the culling type to something other than renderer. Animations may not play
            //if animation.cullingType is left on BasedOnRenderers. This appears to be a bug in Unity
            //the animation gets confused about the bounds if the skinnedMeshRenderer is changed
            anim.cullingType = AnimationCullingType.AlwaysAnimate; //IMPORTANT
            anim.Play("run");

            //create an array with everything we want to add
            //It is important to add the gameObject with the Renderer/mesh attached
            List<GameObject> objsToCombine = skinnedMeshBaker.GetObjectsToCombine();
            GameObject[] objsToAdd = new GameObject[objsToCombine.Count + 1];
            objsToCombine.CopyTo(objsToAdd, 0);
            objsToAdd[objsToAdd.Length - 1] = worker1.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;

            skinnedMeshBaker.ClearMesh();

            //add the objects. This will disable the renderers on the source objects
            skinnedMeshBaker.AddDeleteGameObjects(objsToAdd, null, true);
            //apply the changes to the mesh
            skinnedMeshBaker.Apply();
        }

        void OnGUI()
        {
            if (GUILayout.Button("Add/Remove Sword"))
            {
                if (swordInstance == null)
                {
                    Transform hand = SearchHierarchyForBone(targetCharacter.transform, "RightHandAttachPoint");
                    swordInstance = (GameObject)Instantiate(swordPrefab);
                    swordInstance.transform.parent = hand;
                    swordInstance.transform.localPosition = Vector3.zero;
                    swordInstance.transform.localRotation = Quaternion.identity;
                    swordInstance.transform.localScale = Vector3.one;
                    MeshRenderer swordRenderer = swordInstance.GetComponentInChildren<MeshRenderer>();
                    swordRenderer.gameObject.name = "Sword";
                    GameObject[] objsToAdd = new GameObject[1] { swordRenderer.gameObject };
                    skinnedMeshBaker.AddDeleteGameObjects(objsToAdd, null, true);
                    skinnedMeshBaker.Apply();
                    Debug.Log("Done adding sword.");
                }
                else if (skinnedMeshBaker.CombinedMeshContains(swordInstance.GetComponentInChildren<MeshRenderer>().gameObject))
                {
                    GameObject[] objsToDelete = new GameObject[1] { swordInstance.GetComponentInChildren<MeshRenderer>().gameObject };
                    skinnedMeshBaker.AddDeleteGameObjects(null, objsToDelete, true);
                    skinnedMeshBaker.Apply();
                    Destroy(swordInstance);
                    Debug.Log("Done deleting sword.");
                    swordInstance = null;
                }
            }
            if (GUILayout.Button("Add/Remove Hat"))
            {
                if (hatInstance == null)
                {
                    Transform hand = SearchHierarchyForBone(targetCharacter.transform, "HeadAttachPoint");
                    hatInstance = (GameObject)Instantiate(hatPrefab);
                    hatInstance.transform.parent = hand;
                    hatInstance.transform.localPosition = Vector3.zero;
                    hatInstance.transform.localRotation = Quaternion.identity;
                    hatInstance.transform.localScale = Vector3.one;
                    MeshRenderer hatRenderer = hatInstance.GetComponentInChildren<MeshRenderer>();
                    hatRenderer.gameObject.name = "Hat";
                    GameObject[] objsToAdd = new GameObject[1] { hatRenderer.gameObject };
                    skinnedMeshBaker.AddDeleteGameObjects(objsToAdd, null, true);
                    skinnedMeshBaker.Apply();
                    Debug.Log("Done adding Hat");
                }
                else if (skinnedMeshBaker.CombinedMeshContains(hatInstance.GetComponentInChildren<MeshRenderer>().gameObject))
                {
                    GameObject[] objsToDelete = new GameObject[1] { hatInstance.GetComponentInChildren<MeshRenderer>().gameObject };
                    skinnedMeshBaker.AddDeleteGameObjects(null, objsToDelete, true);
                    skinnedMeshBaker.Apply();
                    Destroy(hatInstance);
                    Debug.Log("Done deleting Hat");
                    hatInstance = null;
                }
            }
            if (GUILayout.Button("Add/Remove Glasses"))
            {
                if (glassesInstance == null)
                {
                    Transform hand = SearchHierarchyForBone(targetCharacter.transform, "NoseAttachPoint");
                    glassesInstance = (GameObject)Instantiate(glassesPrefab);
                    glassesInstance.transform.parent = hand;
                    glassesInstance.transform.localPosition = Vector3.zero;
                    glassesInstance.transform.localRotation = Quaternion.identity;
                    glassesInstance.transform.localScale = Vector3.one;
                    MeshRenderer glassesRenderer = glassesInstance.GetComponentInChildren<MeshRenderer>();
                    glassesRenderer.gameObject.name = "Glasses";
                    GameObject[] objsToAdd = new GameObject[1] { glassesRenderer.gameObject };
                    skinnedMeshBaker.AddDeleteGameObjects(objsToAdd, null, true);
                    skinnedMeshBaker.Apply();
                    Debug.Log("Done adding glasses");
                }
                else if (skinnedMeshBaker.CombinedMeshContains(glassesInstance.GetComponentInChildren<MeshRenderer>().gameObject))
                {
                    GameObject[] objsToDelete = new GameObject[1] { glassesInstance.GetComponentInChildren<MeshRenderer>().gameObject };
                    skinnedMeshBaker.AddDeleteGameObjects(null, objsToDelete, true);
                    skinnedMeshBaker.Apply();
                    Destroy(glassesInstance);
                    glassesInstance = null;
                    Debug.Log("Done deleting glasses");
                }
            }
        }


        public Transform SearchHierarchyForBone(Transform current, string name)
        {
            if (current.name.Equals(name))
                return current;

            for (int i = 0; i < current.childCount; ++i)
            {
                Transform found = SearchHierarchyForBone(current.GetChild(i), name);

                if (found != null)
                    return found;
            }
            return null;
        }
    }
}
