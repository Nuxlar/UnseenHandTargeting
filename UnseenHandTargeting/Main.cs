using BepInEx;
using UnityEngine;
using EntityStates.Seeker;
using RoR2;
using RoR2.UI;

namespace UnseenHandTargeting
{
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class Main : BaseUnityPlugin
  {
    public const string PluginGUID = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "Nuxlar";
    public const string PluginName = "UnseenHandTargeting";
    public const string PluginVersion = "1.0.3";

    internal static Main Instance { get; private set; }
    public static string PluginDirectory { get; private set; }

    public void Awake()
    {
      Instance = this;

      Log.Init(Logger);

      On.EntityStates.Seeker.UnseenHand.UpdateAreaIndicator += EnhanceAim;
    }

    private void EnhanceAim(On.EntityStates.Seeker.UnseenHand.orig_UpdateAreaIndicator orig, UnseenHand self)
    {
      if (!(bool)self.areaIndicatorInstance)
        return;
      int num1 = self.goodPlacement ? 1 : 0;
      self.goodPlacement = false;
      self.areaIndicatorInstance.SetActive(true);
      float maxDistance = UnseenHand.maxDistance;
      float extraRaycastDistance = 0.0f;
      RaycastHit hitInfo;
      if (Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(self.GetAimRay(), self.gameObject, out extraRaycastDistance), out hitInfo, maxDistance + extraRaycastDistance, LayerIndex.CommonMasks.bullet))
      {
        Vector3 position = RaycastToFloor(hitInfo.point);
        self.areaIndicatorInstance.transform.position = position;
        self.goodPlacement = true;
        // self.goodPlacement = (double)Vector3.Angle(Vector3.up, hitInfo.normal) < UnseenHand.maxSlopeAngle;
      }
      int num2 = self.goodPlacement ? 1 : 0;
      if (num1 != num2 || self.crosshairOverrideRequest == null)
      {
        self.crosshairOverrideRequest?.Dispose();
        self.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(self.characterBody, self.goodPlacement ? UnseenHand.goodCrosshairPrefab : UnseenHand.badCrosshairPrefab, CrosshairUtils.OverridePriority.Skill);
      }
      self.areaIndicatorInstance.SetActive(self.goodPlacement);
    }

    private Vector3 RaycastToFloor(Vector3 position)
    {
      RaycastHit hitInfo;
      return Physics.Raycast(new Ray(position, Vector3.down), out hitInfo, 200f, (int)LayerIndex.world.mask, QueryTriggerInteraction.Ignore) ? hitInfo.point : position;
    }

  }
}