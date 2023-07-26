using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformSoloEventTypeUIController : MonoBehaviour, CMInput.IPlatformSoloLightGroupActions
{
    [SerializeField] private TextMeshProUGUI soloEventTypeLabel;
    private PlatformDescriptor descriptor;

    private void Start() => LoadInitialMap.PlatformLoadedEvent += PlatformLoaded;

    private void OnDestroy() => LoadInitialMap.PlatformLoadedEvent -= PlatformLoaded;

    public void OnSoloEventType(InputAction.CallbackContext context)
    {
        if (context.performed) UpdateSoloEventType();
    }

    private void PlatformLoaded(PlatformDescriptor obj) => descriptor = obj;

    public void UpdateSoloEventType() =>
        PersistentUI.Instance.ShowInputBox("Please enter the Event Type or its label.", HandleUpdateSoloEventType);

    private void HandleUpdateSoloEventType(string res)
    {

        if (string.IsNullOrEmpty(res) || string.IsNullOrWhiteSpace(res))
        {
            descriptor.UpdateSoloEventType(false, 0);
        }
        else if (int.TryParse(res, out var id))
        {
            if (id >= 0 && id < descriptor.LightingManagers.Count)
                descriptor.UpdateSoloEventType(true, id);
        }
        else
        {
            var lm = descriptor.LightingManagers.Where(x => x.Value.name == res);
            if (lm.Any())
                descriptor.UpdateSoloEventType(true, lm.First().Key);
        }

        soloEventTypeLabel.gameObject.SetActive(descriptor.SoloAnEventType);
        soloEventTypeLabel.text = $"Soloing <u>{descriptor.LightingManagers[descriptor.SoloEventType].name}</u>";
    }
}
