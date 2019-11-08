//Put both of these scripts in your Project, then go to your Timeline, click the Add dropdown and choose Playable Track. Place this script on your Timeline as a Playable Track
//Click on the track and choose a GameObject from your Scene in the "My Scene Object" field. Also set the velocity.

using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class ExposedReferenceExample : PlayableAsset
{
    //This allows you to use GameObjects in your Scene
    public ExposedReference<GameObject> m_MySceneObject;
    //This variable allows you to decide the velocity of your GameObject
    public Vector3 m_SceneObjectVelocity;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject myGameObject)
    {
        //Get access to the Playable Behaviour script
        TestExample playableBehaviour = new TestExample();
        //Resolve the exposed reference on the Scene GameObject by returning the table used by the graph
        playableBehaviour.m_MySceneObject = m_MySceneObject.Resolve(graph.GetResolver());

        //Make the PlayableBehaviour velocity variable the same as the variable you set
        playableBehaviour.m_SceneObjectVelocity = m_SceneObjectVelocity;

        //Create a custom playable from this script using the Player Behaviour script
        return ScriptPlayable<TestExample>.Create(graph, playableBehaviour);
    }
}
