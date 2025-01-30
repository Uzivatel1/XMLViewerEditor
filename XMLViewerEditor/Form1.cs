using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace XMLViewerEditor
{
    public partial class Form1 : Form
    {
        private XmlDocument xmlDoc = new XmlDocument(); // Objekt pro práci s XML dokumentem.
        private string currentFilePath = string.Empty; // Cesta k aktuálnì otevøenému XML souboru.

        public Form1()
        {
            InitializeComponent();
            treeView.LabelEdit = true; // Umožòuje editaci názvù uzlù v TreeView.

            // Pøiøazení událostí tlaèítkùm.
            openButton.Click += (s, e) => OpenXmlFile(); // Otevøení souboru.
            saveButton.Click += (s, e) => SaveXmlFile(); // Uložení souboru.
            closeButton.Click += (s, e) => ClearAll(); // Vyprázdnìní obsahu.

            // Události TreeView.
            treeView.AfterSelect += (s, e) => DisplayNodeInfo(e.Node); // Zobrazení informací o vybraném uzlu.
            treeView.AfterLabelEdit += TreeView_AfterLabelEdit; // Reakce na zmìnu názvu uzlu.
        }

        // Otevøe XML soubor a naète ho do TreeView.
        private void OpenXmlFile()
        {
            using var dialog = new OpenFileDialog { Filter = "XML soubory (*.xml)|*.xml|Všechny soubory (*.*)|*.*" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    xmlDoc.Load(dialog.FileName); // Naètení XML souboru.
                    currentFilePath = dialog.FileName; // Uložení cesty k souboru.
                    PopulateTreeView(); // Naplnìní TreeView strukturou XML.
                    DisplayFileInfo(); // Zobrazení informací o souboru.
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba pøi otevøení XML: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClearAll(); // Vyprázdnìní TreeView pøi chybì.
                }
            }
        }

        // Uloží XML soubor s pøípadnými zmìnami.
        private void SaveXmlFile()
        {
            using var dialog = new SaveFileDialog { Filter = "XML files (*.xml)|*.xml" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    xmlDoc.Save(dialog.FileName); // Uložení souboru.
                    MessageBox.Show("Soubor byl úspìšnì uložen.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba pøi uložení XML: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Reakce na pøejmenování uzlu v TreeView.
        private void TreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                // Zamezení prázdných názvù uzlù.
                MessageBox.Show("Název uzlu nemùže být prázdný.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.CancelEdit = true;
                return;
            }

            if (e.Node.Tag is XmlNode oldNode)
            {
                // Vytvoøení nového uzlu s novým názvem.
                var newNode = oldNode.OwnerDocument.CreateElement(e.Label);

                // Kopírování atributù do nového uzlu.
                foreach (XmlAttribute attr in oldNode.Attributes)
                {
                    XmlAttribute newAttr = oldNode.OwnerDocument.CreateAttribute(attr.Name);
                    newAttr.Value = attr.Value;
                    newNode.Attributes.Append(newAttr);
                }

                // Kopírování poduzlù do nového uzlu.
                foreach (XmlNode child in oldNode.ChildNodes)
                    newNode.AppendChild(oldNode.OwnerDocument.ImportNode(child, true));

                // Nahrazení starého uzlu novým v dokumentu.
                if (oldNode.ParentNode != null)
                    oldNode.ParentNode.ReplaceChild(newNode, oldNode);
                else
                    xmlDoc.ReplaceChild(newNode, oldNode);

                e.Node.Tag = newNode; // Aktualizace odkazu na nový uzel.
                UpdateChildNodeTags(e.Node, newNode); // Rekurzivní aktualizace poduzlù.
            }
        }

        // Rekurzivnì aktualizuje odkazy na XmlNode u všech poduzlù v TreeView.
        private void UpdateChildNodeTags(TreeNode treeNode, XmlNode xmlNode)
        {
            for (int i = 0; i < treeNode.Nodes.Count; i++)
            {
                treeNode.Nodes[i].Tag = xmlNode.ChildNodes[i];
                UpdateChildNodeTags(treeNode.Nodes[i], xmlNode.ChildNodes[i]);
            }
        }

        // Naplní TreeView strukturou XML.
        private void PopulateTreeView()
        {
            treeView.Nodes.Clear(); // Vyprázdnìní TreeView.
            var rootNode = CreateTreeNode(xmlDoc.DocumentElement); // Vytvoøení koøenového uzlu.
            treeView.Nodes.Add(rootNode); // Pøidání koøenového uzlu.
            AddChildNodes(xmlDoc.DocumentElement, rootNode); // Pøidání poduzlù.
            treeView.ExpandAll(); // Rozbalení všech uzlù.
        }

        // Vytvoøí nový TreeNode pro daný XmlNode.
        private TreeNode CreateTreeNode(XmlNode xmlNode) =>
            new(xmlNode.Name)
            {
                Tag = xmlNode,
                ImageKey = xmlNode.HasChildNodes ? "folder.png" : "file.png", // Rozlišení ikon podle typu uzlu.
                SelectedImageKey = xmlNode.HasChildNodes ? "folder.png" : "file.png"
            };

        // Rekurzivnì pøidává poduzly do TreeView.
        private void AddChildNodes(XmlNode xmlNode, TreeNode parentNode)
        {
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text) continue; // Textové uzly se ignorují.
                var childTreeNode = CreateTreeNode(child);
                parentNode.Nodes.Add(childTreeNode);
                AddChildNodes(child, childTreeNode); // Rekurzivní pøidání poduzlù.
            }

            // Pokud uzel nemá poduzly, nastaví se ikona na "soubor".
            if (parentNode.Nodes.Count == 0)
            {
                parentNode.ImageKey = "file.png";
                parentNode.SelectedImageKey = "file.png";
            }
        }

        // Vyprázdní TreeView a resetuje stav aplikace.
        private void ClearAll()
        {
            treeView.Nodes.Clear();
            fileInfoLabel.Text = elementInfoLabel.Text = string.Empty;
            currentFilePath = string.Empty;
        }

        // Zobrazí informace o vybraném uzlu v TreeView.
        private void DisplayNodeInfo(TreeNode node)
        {
            if (node.Tag is XmlNode selectedNode)
            {
                if (selectedNode.NodeType == XmlNodeType.Element)
                {
                    var element = XElement.Parse(selectedNode.OuterXml); // Parsování do XElement pro snadnou práci.
                    elementInfoLabel.Text = $"Hloubka: {node.Level}\nPoøadí: {node.Parent?.Nodes.IndexOf(node) + 1 ?? 0}\n" +
                                            $"Atributy: {string.Join(", ", element.Attributes().Select(a => $"{a.Name}=\"{a.Value}\""))}\nText: {element.Value.Trim()}";
                }
                else if (selectedNode.NodeType == XmlNodeType.Text)
                {
                    elementInfoLabel.Text = $"Hloubka: {node.Level}\nPoøadí: {node.Parent?.Nodes.IndexOf(node) + 1 ?? 0}\nText: {selectedNode.Value.Trim()}";
                }
            }
        }

        // Zobrazí informace o naèteném XML souboru.
        private void DisplayFileInfo()
        {
            var root = XElement.Load(currentFilePath);
            fileInfoLabel.Text = $"Soubor: {Path.GetFileName(currentFilePath)}\n" +
                                 $"Maximální hloubka: {root.Descendants().Max(e => e.Ancestors().Count())}\n" +
                                 $"Maximální poèet potomkù: {root.Descendants().Max(e => e.Elements().Count())}\n" +
                                 $"Minimální poèet atributù: {root.DescendantsAndSelf().Min(e => e.Attributes().Count())}\n" +
                                 $"Maximální poèet atributù: {root.DescendantsAndSelf().Max(e => e.Attributes().Count())}";
        }
    }
}