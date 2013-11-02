// Copyright (c) 2012 Eric Salczynski and Ramón Rocha
// This program (Kindred Sprite) is released under the MIT License.
// http://opensource.org/licenses/MIT

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using XmlExtensions;

[ExecuteInEditMode]

/// <summary>
/// Handles the import of sprite sheet and atlas data.
/// The spriteData is used by the Sprite and AnimatedSprite classes.
/// 
/// The material has to be shared between game objects, and if it's
/// created and stored in a variable on the SpriteContainer object,
/// which lives in the hierarchy, then there's no need to create
/// new instances of the material.
/// 
/// Shared materials are one of the things that trigger Unity's
/// dynamic batching.
/// - http://docs.unity3d.com/Documentation/Manual/DrawCallBatching.html
/// </summary>
public class SpriteContainer : MonoBehaviour {
 
    /* *** Member Variables *** */
 
    public Texture texture;
    public SpriteData[] spriteData;
    [OnChange ("UpdateAtlasDataFile", typeof(TextAsset))] public TextAsset atlasDataFile = null;
    [OnChange ("UpdateReloadData")] public bool reloadData = false;

    private Material _material;
    private XmlNode _subTexture = null;

    // We don't want to show any of these in the editor, but we want them to be
    // saved on the GameObject, and get serialized properly when starting the player.
    [HideInInspector] public Vector3[] vertices = new Vector3[4];    // 4 coords for upper-left, lower-left, lower-right, upper-right
    [HideInInspector] public int[] triangles = new int[6];           // define the triangles of the mesh using the vertex indices - we're winding clockwise
    [HideInInspector] public Vector3[] normals = new Vector3[4];
    
    /* *** Properties *** */
 
    /// <summary>
    /// Gets the Material object that all sprites that use this SpriteContainer will share.
    /// </summary>
    /// <value>
    /// The Material.
    /// </value>
    public Material material {
        get {
            if (_material == null) {
                _material = new Material(Shader.Find("Sprite"));
            }
            
            if (texture != null) {
                _material.mainTexture = texture;
            }
            
            return _material;
        }
    }
 
    /* *** Public Methods *** */
    
    /// <summary>
    /// Since the verts/tris/normals should be exactly the same for all sprites in the container,
    /// we can keep that info on the SpriteContainer, rather than duplicate it for each SpriteData
    /// object in our array.
    /// </summary>
    public void InitVertices() {
        UpdateVertices(0);

        // also update the triangles - Clockwise winding
        triangles[0] = 0;        //  2               2 ___ 3
        triangles[1] = 2;        //  |\        Verts: |\  |
        triangles[2] = 1;        // 0|_\1            0|_\|1
 
        triangles[3] = 2;        // 3 __ 4
        triangles[4] = 3;        //   \ |
        triangles[5] = 1;        //    \|5

        // and finally, update the normals. Since we know we're in XY 2D space,
        // We can just make the normals face forward, and save some computing time.
        normals[0] = Vector3.forward;
        normals[1] = Vector3.forward;
        normals[2] = Vector3.forward;
        normals[3] = Vector3.forward;
    }
    
    /// <summary>
    /// Updates the vertices for a centered pivot point.
    /// The z-depth of the verts are the only variable component.
    /// </summary>
    /// <param name='depth'>
    /// Z-depth of the verts.
    /// </param>
    public void UpdateVertices(float depth) {
        vertices[0] = new Vector3(-0.5f, -0.5f, depth);   // lower-left
        vertices[1] = new Vector3(0.5f, -0.5f, depth);    // lower-right
        vertices[2] = new Vector3(-0.5f, 0.5f, depth);    // upper-left
        vertices[3] = new Vector3(0.5f, 0.5f, depth);     // upper-right
    }
    
    /// <summary>
    /// Callback for OnChange event from the Unity editor.
    /// Reload the sprite data when the atlas data file changes.
    /// </summary>
    /// <param name='newVal'>
    /// New value for `atlasDataFile`
    /// </param>
    public void UpdateAtlasDataFile(TextAsset newVal) {
        atlasDataFile = newVal;
        ReloadData();
    }
    
    /// <summary>
    /// Callback for OnChange event from the Unity editor.
    /// Forces a reload of the sprite data.
    /// </summary>
    /// <param name='newVal'>
    /// Boolean
    /// </param>
    public void UpdateReloadData(bool newVal) {
        ReloadData();
    }
    
    /* *** Private Methods *** */
    
    /// <summary>
    /// Read and parse atlas data from XML file.
    /// </summary>
    private void ImportAtlasData() {
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(atlasDataFile.text);
        XmlNode frames = xml.DocumentElement.SelectSingleNode("dict/key");
        List<AtlasData> data = new List<AtlasData>();
        
        if (frames != null && frames.InnerText == "frames") {
            XmlNodeList subTextureNames = xml.DocumentElement.SelectNodes("dict/dict/key");
            XmlNodeList subTextures = xml.DocumentElement.SelectNodes("dict/dict/dict");
            try {
                for (int si = 0; si < subTextures.Count; si++) {
                    _subTexture = subTextures[si];
                    AtlasData ad = new AtlasData();

                    bool rotated = _subTexture.GetBool("rotated");
                    Rect frame = _subTexture.GetRect("frame");
                    Rect colorRect = _subTexture.GetRect("sourceColorRect");
                    Vector2 sourceSize = _subTexture.GetVector2("sourceSize");
                    
                    try {
                        ad.name = subTextureNames[si].InnerText.Split('.')[0];
                    } catch (System.Exception) {
                        ad.name = subTextureNames[si].InnerText;
                    }
                    ad.position = new Vector2(frame.xMin, frame.yMin);
                    ad.rotated = rotated;
                    ad.size = new Vector2(colorRect.width, colorRect.height);
                    ad.frameSize = sourceSize;
                    ad.offset = new Vector2(colorRect.xMin, colorRect.yMin);
                    
                    data.Add(ad);
                }
            } catch (System.Exception ERR) {
                Debug.LogError("Atlas Import error!");
                Debug.LogError(ERR.Message);
            }
        }
        
        InitVertices();
        spriteData = new SpriteData[data.Count];
        SpriteData sprite = null;
        for (int i = 0; i < data.Count; i++) {
            sprite = new SpriteData();
            sprite.name = data[i].name;
            sprite.size = data[i].size;
            sprite.sheetPixelCoords = data[i].position;
            sprite.texture = texture;
            sprite.UpdateUVs();
            
            spriteData[i] = sprite;
        }
    }
    
    /// <summary>
    /// Reloads the sprite data.
    /// </summary>
    private void ReloadData() {
        if (atlasDataFile != null) {
            ImportAtlasData();
        }

        reloadData = false;

        #if UNITY_EDITOR
            if (!Application.isPlaying) {
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        #endif
    }
}
