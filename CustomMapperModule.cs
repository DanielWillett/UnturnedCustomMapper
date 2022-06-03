using SDG.Framework.Modules;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomMapper;

public class CustomMapperModule : IModuleNexus
{
    private bool init = false;
    private SleekButtonIcon customMapperButton = null!;
    private CustomMapperUI ui = null!;
    private GameObject parent = null!;
    void IModuleNexus.initialize()
    {
        Level.onLevelLoaded += OnLevelLoaded;
    }
    void IModuleNexus.shutdown()
    {
        init = false;
        if (customMapperButton is not null)
        {
            try
            {
                if (parent != null)
                    UnityEngine.Object.Destroy(parent);
                object? obj = typeof(EditorPauseUI).GetField("container", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
                if (obj is SleekFullscreenBox box)
                {
                    box.RemoveChild(customMapperButton);
                }
            }
            catch
            { }
            customMapperButton = null!;
            parent = null!;
        }
        Level.onLevelLoaded -= OnLevelLoaded;
    }
    private void OnLevelLoaded(int level)
    {
        object? obj = typeof(EditorPauseUI).GetField("container", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
        if (obj is SleekFullscreenBox box && (customMapperButton == null || box.FindIndexOfChild(customMapperButton) == -1))
        {
            init = true;
            parent = new GameObject("CustomMapper");
            UnityEngine.Object.DontDestroyOnLoad(parent);
            ui = parent.AddComponent<CustomMapperUI>();
            Bundle bundle1 = Bundles.getBundle("/Bundles/Textures/Edit/Icons/EditorPause/EditorPause.unity3d");
            customMapperButton = new SleekButtonIcon(bundle1.load<Texture2D>("Chart"));
            bundle1.unload();
            customMapperButton.positionOffset_X = -100;
            customMapperButton.positionOffset_Y = -155;
            customMapperButton.positionScale_X = 0.5f;
            customMapperButton.positionScale_Y = 0.5f;
            customMapperButton.sizeOffset_X = 200;
            customMapperButton.sizeOffset_Y = 30;
            customMapperButton.text = "Custom Mapper";
            customMapperButton.onClickedButton += OnCustomMapperButtonClicked;
            box.AddChild(customMapperButton);
            UnturnedLog.info("[CUSTOM MAPPER] Added button");
        }

    }

    private void OnCustomMapperButtonClicked(ISleekElement button)
    {
        EditorPauseUI.close();
        CustomMapperUI.Open();
    }
}
