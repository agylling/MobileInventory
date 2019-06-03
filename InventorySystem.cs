using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{

    private string filePath;
    // Select a name for the textfile database 
    private string InventoryFileName = "Inventory.txt";

    public Dictionary<string, int> Inventory = new Dictionary<string, int>();

    public int fontSize = 80;

    // Start is called before the first frame update
    void Start()
    {
        // The path that is used to read and save to file. PersistentDatapath = readOnly
        filePath = Application.persistentDataPath + "/" + InventoryFileName;
        LoadInventory();
    }


    /**
     * Loads the data from a text file.
     */
    private void LoadInventory()
    {
        if (!File.Exists(filePath)) // Check if the installation has a file
        {
            // Create a file
            FileStream file = File.Create(Application.persistentDataPath + InventoryFileName);
            file.Close();
            // Loading all Sprite (items) I want in my default database text file
            loadAssetsToDB();
        }
        // Loading the contents of the database file
        string fileData = File.ReadAllText(filePath);
        // Items are split by a , (Do not use this character in any asset, or change this split)
        string[] items = fileData.Split(',');
        for (int i = 0; i < items.Length; i++)
        {
            // Enter the current item in runtime inventory data structure 
            FillMap(items[i]);
        }
    }

    /**
     * Saves the runtime inventory to a textfile database.
     */
    public void SaveToFile()
    {
        // Reset File so it becomes an empty file.
        File.WriteAllText(Application.persistentDataPath + "/" + InventoryFileName, "");
        bool firstEntry = true;
        // Iterate through runtime inventory
        foreach (KeyValuePair<string, int> item in Inventory)
        {
            // If it's the first item to be appended to file, no need for a , at start. (Will cause error otherwise)
            string currentItem = firstEntry ? "" : ",";
            currentItem += item.Key + ":" + item.Value;
            // Append item to file
            File.AppendAllText(Application.persistentDataPath + "/" + InventoryFileName, currentItem);
            if (firstEntry) firstEntry = false;
        }
    }

    public void loadAssetsToDB()
    {
        // Load an entire folder of sprites to add to the database text file. Change the first parameter to your path inside the Resources folder
        Object[] loadedSprites = Resources.LoadAll("Skipan'sToonyIcons/DarkAndFrame", typeof(Sprite));
        for (int i = 0; i < loadedSprites.Length; i++)
        {
            // If the map doesnt already contain current key, add it with default value 0
            if (!(Inventory.ContainsKey(loadedSprites[i].name)))
            {
                Inventory.Add(loadedSprites[i].name, 0);
            }
        }
        SaveToFile();
    }

    /**
     * From the data read from file,
     * fill out the runtime Inventory map.
     */
    private void FillMap(string item)
    {
        // An asset (item) is typed as <key>:<value>, split key and value by :
        string[] tuple = item.Split(':');
        // See if the item entry exists in the database before trying to access it
        checkExistance(tuple[0]);
        Inventory[tuple[0]] = int.Parse(tuple[1]);
        GameObject tmp = GameObject.Find(tuple[0]);
        if (tmp != null) UpdateGUI(tmp);
    }

    /**
     * Updates the item count showed on the GUI.
     * par item - The icon seen on screen
     */
    public void UpdateGUI(GameObject item)
    {
        // Check if there's a UI element that shows item counter
        GameObject itemText = GameObject.Find(item.name + "_count");
        if (itemText == null)
        {
            // If not, create the Text object.
            GameObject counter = new GameObject();
            counter.name = item.name + "_count";
            counter.AddComponent<Text>();
            Text text = counter.GetComponent<Text>();
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleLeft;
            text.text = Inventory[item.name].ToString();
            counter.transform.position = item.transform.position;
            counter.transform.position += new Vector3(150, 0, 0);

            // Set the item to be the parent of the text object
            counter.transform.SetParent(item.transform);
        }
        else
        {
            Text text = itemText.GetComponent<Text>();
            // Set the text value to the counter of the item in Inventory
            text.text = Inventory[item.name].ToString();
        }
    }

    private void checkExistance(string itemName)
    {
        if (Inventory.ContainsKey(itemName) == false)
        {
            // Add it to the Inventory
            Inventory.Add(itemName, 0);
        }
    }

    /**
     * Increment parameter item by 1, used by buttons
     */
    public void IncrementItem(GameObject item)
    {
        checkExistance(item.name);
        Inventory[item.name] += 1;
        UpdateGUI(item);
        SaveToFile();
    }

    /**
    * Decrease parameter item by 1, used by buttons
    */
    public void DecreaseItem(GameObject item)
    {
        checkExistance(item.name);
        Inventory[item.name] -= 1;
        UpdateGUI(item);
        SaveToFile();
    }

    /**
     * Increase the total amount of item X in the database.
     */
    public void IncreaseItem(GameObject item, int amount)
    {
        checkExistance(item.name);
        Inventory[item.name] += amount;
        UpdateGUI(item);
        SaveToFile();
    }
}