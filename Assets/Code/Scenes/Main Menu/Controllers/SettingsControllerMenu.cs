using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SettingsControllerMenu : MonoBehaviour {
    Camera mainCamera;
    Terrain terrainThing;

    void Start () {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        terrainThing = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
        terrainThing.treeDistance = 3000f;
        terrainThing.treeBillboardDistance = 3000f;
        mainCamera.farClipPlane = 3000f;
        terrainThing.detailObjectDistance = 1000f;
        QualitySettings.shadowDistance = 1000f;
    }
    
    void Update () {
        
    }

    public void SetDrawDistance(Slider slider) {
        mainCamera.farClipPlane = slider.value;
        slider.GetComponentInChildren<Text>().text = Mathf.Round(slider.value).ToString();
    }

    public void SetTreeDistance(Slider slider) {
        terrainThing.treeDistance = slider.value;
        terrainThing.treeBillboardDistance = slider.value;
        slider.GetComponentInChildren<Text>().text = Mathf.Round(slider.value).ToString();
    }

    public void SetDetailDistance(Slider slider) {
        terrainThing.detailObjectDistance = slider.value;
        slider.GetComponentInChildren<Text>().text = Mathf.Round(slider.value).ToString();
    }

    public void SetShadowDistance(Slider slider) {
        QualitySettings.shadowDistance = slider.value;
        slider.GetComponentInChildren<Text>().text = Mathf.Round(slider.value).ToString();
    }

    public void OnTextureDetailChanged(Dropdown dropdown) {
        QualitySettings.masterTextureLimit = dropdown.value;
    }

    public void OnAntialiasingLevelChanged(Dropdown dropdown) {
        switch (dropdown.value) {
            case 0:
                QualitySettings.antiAliasing = 0;
                break;
            case 1:
                QualitySettings.antiAliasing = 2;
                break;
            case 2:
                QualitySettings.antiAliasing = 4;
                break;
            case 3:
                QualitySettings.antiAliasing = 8;
                break;
            default:
                break;
        }
    }
}
