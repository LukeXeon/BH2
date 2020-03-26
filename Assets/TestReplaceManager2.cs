using System.Threading.Tasks;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;

public class TestReplaceManager2 : MonoBehaviour
{
    public SkeletonGraphic animation;
    public Sprite sprite;
    public Vector2 offset;
    public string slot;

    public string attacthment;
    public bool use;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void onTest()
    {
        Debug.Log(sprite.textureRect);

        var m = animation.material;
        var old = (RegionAttachment) animation.Skeleton.Data.FindSkin("default")
            .GetAttachment(animation.Skeleton.FindSlotIndex(slot), attacthment);
        var h = sprite.texture.height;
        var w = sprite.texture.width;
        var s = Sprite.Create(sprite.texture, new Rect
            {
                width = w,
                height = h
            }, offset,
            100,
            1,
            SpriteMeshType.FullRect
        );
        Debug.Log(s.textureRect);
        var attachment =
            (RegionAttachment) old.GetRemappedClone(s, m, useOriginalRegionSize: use);

        Skin skin = new Skin("Test");
        skin.Append(animation.Skeleton.Data.DefaultSkin);

        skin.SetAttachment(animation.Skeleton.FindSlotIndex(slot), attacthment, attachment);
        animation.Skeleton.SetSkin(skin);
        RefreshSkeletonAttachments();
    }
    
    void RefreshSkeletonAttachments()
    {
        animation.Skeleton.SetSlotsToSetupPose();
        animation.AnimationState.Apply(animation.Skeleton); //skeletonAnimation.Update(0);
        animation.Initialize(false);
    }
}