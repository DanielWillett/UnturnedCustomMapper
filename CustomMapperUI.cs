using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Action = System.Action;

namespace CustomMapper;
public class CustomMapperUI : MonoBehaviour
{
    public static bool Active;
#nullable disable
    private static SleekFullscreenBox container;
    private static SleekButtonIcon chartifyButton;
    private static SleekButtonIcon mapifyButton;
    private static ISleekButton currentPosButton1;
    private static ISleekButton currentPosButton2;
    private static ISleekButton currentRotButton;
    private static ISleekButton currentLocationButton;
    private static ISleekButton locationSetButton;
    private static ISleekImage preview;
    private static RenderTexture txt;

    private static ISleekFloat32Field posX1Field;
    private static ISleekFloat32Field posY1Field;
    private static ISleekFloat32Field posX2Field;
    private static ISleekFloat32Field posY2Field;
    private static ISleekFloat32Field rotField;
    private static ISleekField locField;
    private static ISleekFloat32Field locRadField;
    //private static ISleekInt32Field resolution;
#nullable restore
    public static void Open()
    {
        UnturnedLog.info("[CUSTOM MAPPER] Open requested");
        if (Active) return;
        Active = true;
        container.AnimateIntoView();
        UnturnedLog.info("[CUSTOM MAPPER] Started open");
    }
    public static void Close()
    {
        UnturnedLog.info("[CUSTOM MAPPER] Close requested");
        if (!Active) return;
        Active = false;
        container.AnimateOutOfView(0f, 1f);
        UnturnedLog.info("[CUSTOM MAPPER] Started close");
    }
    public CustomMapperUI()
    {
        Local local1 = Localization.read("/Editor/EditorPause.dat");
        Bundle bundle1 = Bundles.getBundle("/Bundles/Textures/Edit/Icons/EditorPause/EditorPause.unity3d");
        container = new SleekFullscreenBox
        {
            positionOffset_X = 10,
            positionOffset_Y = 10,
            positionScale_X = 1f,
            sizeOffset_X = -20,
            sizeOffset_Y = -20,
            sizeScale_X = 1f,
            sizeScale_Y = 1f
        };
        EditorUI.window.AddChild(container);
        Active = false;

        rotField = Glazier.Get().CreateFloat32Field();
        rotField.positionOffset_X = -100;
        rotField.positionOffset_Y = 5;
        rotField.positionScale_X = 0.5f;
        rotField.positionScale_Y = 0.5f;
        rotField.sizeOffset_X = 200;
        rotField.sizeOffset_Y = 30;
        rotField.tooltipText = "Rotation";
        container.AddChild(rotField);

        posX1Field = Glazier.Get().CreateFloat32Field();
        posX1Field.positionOffset_X = -100;
        posX1Field.positionOffset_Y = -75;
        posX1Field.positionScale_X = 0.5f;
        posX1Field.positionScale_Y = 0.5f;
        posX1Field.sizeOffset_X = 200;
        posX1Field.sizeOffset_Y = 30;
        posX1Field.tooltipText = "Corner 1 X";
        container.AddChild(posX1Field);

        posY1Field = Glazier.Get().CreateFloat32Field();
        posY1Field.positionOffset_X = 110;
        posY1Field.positionOffset_Y = -75;
        posY1Field.positionScale_X = 0.5f;
        posY1Field.positionScale_Y = 0.5f;
        posY1Field.sizeOffset_X = 200;
        posY1Field.sizeOffset_Y = 30;
        posY1Field.tooltipText = "Corner 1 Y";
        container.AddChild(posY1Field);

        posX2Field = Glazier.Get().CreateFloat32Field();
        posX2Field.positionOffset_X = -100;
        posX2Field.positionOffset_Y = -35;
        posX2Field.positionScale_X = 0.5f;
        posX2Field.positionScale_Y = 0.5f;
        posX2Field.sizeOffset_X = 200;
        posX2Field.sizeOffset_Y = 30;
        posX2Field.tooltipText = "Corner 2 X";
        container.AddChild(posX2Field);

        posY2Field = Glazier.Get().CreateFloat32Field();
        posY2Field.positionOffset_X = 110;
        posY2Field.positionOffset_Y = -35;
        posY2Field.positionScale_X = 0.5f;
        posY2Field.positionScale_Y = 0.5f;
        posY2Field.sizeOffset_X = 200;
        posY2Field.sizeOffset_Y = 30;
        posY2Field.tooltipText = "Corner 2 Y";
        container.AddChild(posY2Field);

        locField = Glazier.Get().CreateStringField();
        locField.positionOffset_X = -100;
        locField.positionOffset_Y = 45;
        locField.positionScale_X = 0.5f;
        locField.positionScale_Y = 0.5f;
        locField.sizeOffset_X = 200;
        locField.sizeOffset_Y = 30;
        locField.tooltipText = "Location";
        container.AddChild(locField);
        /*
        resolution = Glazier.Get().CreateInt32Field();
        resolution.state = 2048;
        resolution.positionOffset_X = 110;
        resolution.positionOffset_Y = -35;
        resolution.positionScale_X = 0.5f;
        resolution.positionScale_Y = 0.5f;
        resolution.sizeOffset_X = 200;
        resolution.sizeOffset_Y = 30;
        resolution.tooltipText = "Resolution XY";
        container.AddChild(resolution);
        */
        chartifyButton = new SleekButtonIcon(bundle1.load<Texture2D>("Chart"));
        chartifyButton.positionOffset_X = -100;
        chartifyButton.positionOffset_Y = -115;
        chartifyButton.positionScale_X = 0.5f;
        chartifyButton.positionScale_Y = 0.5f;
        chartifyButton.sizeOffset_X = 200;
        chartifyButton.sizeOffset_Y = 30;
        chartifyButton.text = local1.format("Chart_Button");
        chartifyButton.onClickedButton += OnClickChartify;
        container.AddChild(chartifyButton);

        mapifyButton = new SleekButtonIcon(bundle1.load<Texture2D>("Map"));
        mapifyButton.positionOffset_X = 110;
        mapifyButton.positionOffset_Y = -115;
        mapifyButton.positionScale_X = 0.5f;
        mapifyButton.positionScale_Y = 0.5f;
        mapifyButton.sizeOffset_X = 200;
        mapifyButton.sizeOffset_Y = 30;
        mapifyButton.text = local1.format("Map_Button");
        mapifyButton.onClickedButton += OnClickMapify;
        container.AddChild(mapifyButton);

        currentPosButton1 = Glazier.Get().CreateButton();
        currentPosButton1.positionOffset_X = 320;
        currentPosButton1.positionOffset_Y = -75;
        currentPosButton1.positionScale_X = 0.5f;
        currentPosButton1.positionScale_Y = 0.5f;
        currentPosButton1.sizeOffset_X = 160;
        currentPosButton1.sizeOffset_Y = 30;
        currentPosButton1.text = "Current Position";
        currentPosButton1.onClickedButton += OnClickCurrentPosition1;
        container.AddChild(currentPosButton1);

        currentPosButton2 = Glazier.Get().CreateButton();
        currentPosButton2.positionOffset_X = 320;
        currentPosButton2.positionOffset_Y = -35;
        currentPosButton2.positionScale_X = 0.5f;
        currentPosButton2.positionScale_Y = 0.5f;
        currentPosButton2.sizeOffset_X = 160;
        currentPosButton2.sizeOffset_Y = 30;
        currentPosButton2.text = "Current Position";
        currentPosButton2.onClickedButton += OnClickCurrentPosition2;
        container.AddChild(currentPosButton2);

        currentRotButton = Glazier.Get().CreateButton();
        currentRotButton.positionOffset_X = 320;
        currentRotButton.positionOffset_Y = 5;
        currentRotButton.positionScale_X = 0.5f;
        currentRotButton.positionScale_Y = 0.5f;
        currentRotButton.sizeOffset_X = 160;
        currentRotButton.sizeOffset_Y = 30;
        currentRotButton.text = "Current Rotation";
        currentRotButton.onClickedButton += OnClickCurrentRotation;
        container.AddChild(currentRotButton);

        currentLocationButton = Glazier.Get().CreateButton();
        currentLocationButton.positionOffset_X = 320;
        currentLocationButton.positionOffset_Y = 45;
        currentLocationButton.positionScale_X = 0.5f;
        currentLocationButton.positionScale_Y = 0.5f;
        currentLocationButton.sizeOffset_X = 160;
        currentLocationButton.sizeOffset_Y = 30;
        currentLocationButton.text = "Current Location";
        currentLocationButton.onClickedButton += OnClickCurrentLocation;
        container.AddChild(currentLocationButton);

        locationSetButton = Glazier.Get().CreateButton();
        locationSetButton.positionOffset_X = 110;
        locationSetButton.positionOffset_Y = 45;
        locationSetButton.positionScale_X = 0.5f;
        locationSetButton.positionScale_Y = 0.5f;
        locationSetButton.sizeOffset_X = 160;
        locationSetButton.sizeOffset_Y = 30;
        locationSetButton.text = "From Distance";
        locationSetButton.onClickedButton += OnClickSetupLocation;
        container.AddChild(locationSetButton);

        preview = Glazier.Get().CreateImage(txt);
        preview.positionOffset_X = -512;
        preview.positionOffset_Y = 0;
        preview.positionScale_X = 0.25f;
        preview.positionScale_Y = 0.25f;
        preview.sizeOffset_X = 1024;
        preview.sizeOffset_Y = 1024;
        container.AddChild(preview);

        bundle1.unload();

        UnturnedLog.info("[CUSTOM MAPPER] Added UI");
    }

    private void OnClickSetupLocation(ISleekElement button)
    {
        string txt = locField.text;
        if (string.IsNullOrEmpty(txt)) return;
        for (int i = 0; i < LevelNodes.nodes.Count; ++i)
        {
            if (LevelNodes.nodes[i] is LocationNode node && node.name.Equals(txt, StringComparison.OrdinalIgnoreCase))
            {
                Load(node);
                return;
            }
        }
        foreach (LocationNode node in LevelNodes.nodes.OfType<LocationNode>().OrderBy(x => x.name.Length))
        {
            if (node.name.IndexOf(txt, StringComparison.OrdinalIgnoreCase) != -1)
            {
                Load(node);
                return;
            }
        }
    }
    private void Load(LocationNode node)
    {
        locField.text = node.name;
        Vector3 pos = Camera.main.gameObject.transform.position;
        float radius = Mathf.Sqrt(Mathf.Pow(pos.x - node.point.x, 2) + Mathf.Pow(pos.z - node.point.z, 2));
        posX1Field.state = node.point.x + radius;
        posY1Field.state = node.point.z + radius;
        posX2Field.state = node.point.x - radius;
        posY2Field.state = node.point.z - radius;
    }
    private void OnClickCurrentLocation(ISleekElement button)
    {
        if (EditorNodes.node is LocationNode node)
        {
            locField.text = node.name;
        }
        else
        {
            int n = -1;
            float sqrDist = float.NaN;
            Vector3 pos = Camera.main.gameObject.transform.position;
            for (int i = 0; i < LevelNodes.nodes.Count; ++i)
            {
                if (LevelNodes.nodes[i] is LocationNode node2)
                {
                    float sqrd = (pos - node2.point).sqrMagnitude;
                    if (float.IsNaN(sqrDist) || sqrDist > sqrd)
                    {
                        sqrDist = sqrd;
                        n = i;
                    }
                }
            }

            if (n == -1)
                locField.text = string.Empty;
            else
                locField.text = ((LocationNode)LevelNodes.nodes[n]).name;
        }
    }

    private void OnClickCurrentPosition1(ISleekElement button)
    {
        posX1Field.state = Camera.main.gameObject.transform.position.x;
        posY1Field.state = Camera.main.gameObject.transform.position.z;
    }

    private void OnClickCurrentPosition2(ISleekElement button)
    {
        posX2Field.state = Camera.main.gameObject.transform.position.x;
        posY2Field.state = Camera.main.gameObject.transform.position.z;
    }

    private void OnClickCurrentRotation(ISleekElement button)
    {
        rotField.state = Camera.main.gameObject.transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        if (Active && InputEx.GetKeyDown(KeyCode.Escape))
        {
            InputEx.ConsumeKeyDown(KeyCode.Escape);
            Close();
        }
    }
    private void OnClickMapify(ISleekElement button)
    {
        UnturnedLog.info("[CUSTOM MAPPER] Mapping");
        Transform? mapper = typeof(Level).GetField("mapper", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as Transform;
        if (mapper != null)
        {
            string mapName = Level.info.path + "/Map.png";
            string destFileName = Level.info.path + "/Map_TEMP.png";

            if (File.Exists(mapName)) // save the old map in a temp file
                File.Copy(mapName, destFileName);

            Vector3 t1 = mapper.position;
            Quaternion t2 = mapper.rotation;
            if (!mapper.TryGetComponent(out Camera camera))
                return;
            float t3 = camera.orthographicSize;

            Vector2 corner1 = new Vector2(posX1Field.state, posY1Field.state);
            Vector2 corner2 = new Vector2(posX2Field.state, posY2Field.state);

            if (corner1.x == corner2.x || corner1.y == corner2.y)
                return;

            // make sure corner1 is the "largest" corner.
            if (corner2.x > corner1.x)
                (corner1.x, corner2.x) = (corner2.x, corner1.x);
            if (corner2.y > corner1.y)
                (corner1.y, corner2.y) = (corner2.y, corner1.y);

            // square size
            camera.orthographicSize = Mathf.Max(corner1.x - corner2.x, corner1.y - corner2.y) / 2;

            mapper.SetPositionAndRotation(new Vector3(corner2.x + (corner1.x - corner2.x) / 2, 1028f, corner2.y + (corner1.y - corner2.y) / 2), Quaternion.Euler(90f, rotField.state, 0f));

            Level.mapify();

            camera.orthographicSize = t3;
            mapper.SetPositionAndRotation(t1, t2);

            if (preview.texture != null)
                Destroy(preview.texture);

            Texture2D txt = new Texture2D(Level.size, Level.size);
            txt.LoadImage(File.ReadAllBytes(mapName));
            preview.texture = txt;


            // save to desktop
            string? str1 = Environment.GetEnvironmentVariable("OneDrive");
            if (str1 is null) str1 = Environment.GetEnvironmentVariable("UserProfile");
            string path = str1 is null ? Level.info.path + "Map_2.png" : Path.Combine(str1, "Desktop", "Map.png");

            File.WriteAllBytes(path, txt.EncodeToPNG());
            File.Delete(mapName);
            if (File.Exists(destFileName))
            {
                File.Copy(destFileName, mapName);
                File.Delete(destFileName);
            }

            UnturnedLog.info("[CUSTOM MAPPER] Success! Saved to \"" + path + "\".");
        }
    }

    private void OnClickChartify(ISleekElement button)
    {
        UnturnedLog.info("[CUSTOM MAPPER] Charting");
        string mapName = Level.info.path + "/Chart.png";
        string destFileName = Level.info.path + "/Chart_TEMP.png";
        if (File.Exists(mapName))
            File.Copy(mapName, destFileName);

        Level.chartify();

        if (preview.texture != null)
            Destroy(preview.texture);
        Texture2D txt = new Texture2D(Level.size, Level.size);
        txt.LoadImage(File.ReadAllBytes(mapName));
        preview.texture = txt;
        File.Delete(mapName);
        string? str1 = Environment.GetEnvironmentVariable("OneDrive");
        if (str1 is null) str1 = Environment.GetEnvironmentVariable("UserProfile");
        string path = str1 is null ? Level.info.path + "Chart_2.png" : Path.Combine(str1, "Desktop", "Chart.png");
        File.WriteAllBytes(path, txt.EncodeToPNG());
        UnturnedLog.info("[CUSTOM MAPPER] Copied to \"" + path + "\"");
        if (File.Exists(destFileName))
        {
            File.Copy(destFileName, mapName);
            File.Delete(destFileName);
        }
        UnturnedLog.info("[CUSTOM MAPPER] Done");
    }
}
