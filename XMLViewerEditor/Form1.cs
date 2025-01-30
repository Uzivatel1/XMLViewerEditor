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
        private XmlDocument xmlDoc = new XmlDocument(); // Objekt pro pr�ci s XML dokumentem.
        private string currentFilePath = string.Empty; // Cesta k aktu�ln� otev�en�mu XML souboru.

        public Form1()
        {
            InitializeComponent();
            treeView.LabelEdit = true; // Umo��uje editaci n�zv� uzl� v TreeView.

            // P�i�azen� ud�lost� tla��tk�m.
            openButton.Click += (s, e) => OpenXmlFile(); // Otev�en� souboru.
            saveButton.Click += (s, e) => SaveXmlFile(); // Ulo�en� souboru.
            closeButton.Click += (s, e) => ClearAll(); // Vypr�zdn�n� obsahu.

            // Ud�losti TreeView.
            treeView.AfterSelect += (s, e) => DisplayNodeInfo(e.Node); // Zobrazen� informac� o vybran�m uzlu.
            treeView.AfterLabelEdit += TreeView_AfterLabelEdit; // Reakce na zm�nu n�zvu uzlu.
        }

        // Otev�e XML soubor a na�te ho do TreeView.
        private void OpenXmlFile()
        {
            using var dialog = new OpenFileDialog { Filter = "XML soubory (*.xml)|*.xml|V�echny soubory (*.*)|*.*" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    xmlDoc.Load(dialog.FileName); // Na�ten� XML souboru.
                    currentFilePath = dialog.FileName; // Ulo�en� cesty k souboru.
                    PopulateTreeView(); // Napln�n� TreeView strukturou XML.
                    DisplayFileInfo(); // Zobrazen� informac� o souboru.
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba p�i otev�en� XML: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClearAll(); // Vypr�zdn�n� TreeView p�i chyb�.
                }
            }
        }

        // Ulo�� XML soubor s p��padn�mi zm�nami.
        private void SaveXmlFile()
        {
            using var dialog = new SaveFileDialog { Filter = "XML files (*.xml)|*.xml" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    xmlDoc.Save(dialog.FileName); // Ulo�en� souboru.
                    MessageBox.Show("Soubor byl �sp�n� ulo�en.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba p�i ulo�en� XML: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Reakce na p�ejmenov�n� uzlu v TreeView.
        private void TreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                // Zamezen� pr�zdn�ch n�zv� uzl�.
                MessageBox.Show("N�zev uzlu nem��e b�t pr�zdn�.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.CancelEdit = true;
                return;
            }

            if (e.Node.Tag is XmlNode oldNode)
            {
                // Vytvo�en� nov�ho uzlu s nov�m n�zvem.
                var newNode = oldNode.OwnerDocument.CreateElement(e.Label);

                // Kop�rov�n� atribut� do nov�ho uzlu.
                foreach (XmlAttribute attr in oldNode.Attributes)
                {
                    XmlAttribute newAttr = oldNode.OwnerDocument.CreateAttribute(attr.Name);
                    newAttr.Value = attr.Value;
                    newNode.Attributes.Append(newAttr);
                }

                // Kop�rov�n� poduzl� do nov�ho uzlu.
                foreach (XmlNode child in oldNode.ChildNodes)
                    newNode.AppendChild(oldNode.OwnerDocument.ImportNode(child, true));

                // Nahrazen� star�ho uzlu nov�m v dokumentu.
                if (oldNode.ParentNode != null)
                    oldNode.ParentNode.ReplaceChild(newNode, oldNode);
                else
                    xmlDoc.ReplaceChild(newNode, oldNode);

                e.Node.Tag = newNode; // Aktualizace odkazu na nov� uzel.
                UpdateChildNodeTags(e.Node, newNode); // Rekurzivn� aktualizace poduzl�.
            }
        }

        // Rekurzivn� aktualizuje odkazy na XmlNode u v�ech poduzl� v TreeView.
        private void UpdateChildNodeTags(TreeNode treeNode, XmlNode xmlNode)
        {
            for (int i = 0; i < treeNode.Nodes.Count; i++)
            {
                treeNode.Nodes[i].Tag = xmlNode.ChildNodes[i];
                UpdateChildNodeTags(treeNode.Nodes[i], xmlNode.ChildNodes[i]);
            }
        }

        // Napln� TreeView strukturou XML.
        private void PopulateTreeView()
        {
            treeView.Nodes.Clear(); // Vypr�zdn�n� TreeView.
            var rootNode = CreateTreeNode(xmlDoc.DocumentElement); // Vytvo�en� ko�enov�ho uzlu.
            treeView.Nodes.Add(rootNode); // P�id�n� ko�enov�ho uzlu.
            AddChildNodes(xmlDoc.DocumentElement, rootNode); // P�id�n� poduzl�.
            treeView.ExpandAll(); // Rozbalen� v�ech uzl�.
        }

        // Vytvo�� nov� TreeNode pro dan� XmlNode.
        private TreeNode CreateTreeNode(XmlNode xmlNode) =>
            new(xmlNode.Name)
            {
                Tag = xmlNode,
                ImageKey = xmlNode.HasChildNodes ? "folder.png" : "file.png", // Rozli�en� ikon podle typu uzlu.
                SelectedImageKey = xmlNode.HasChildNodes ? "folder.png" : "file.png"
            };

        // Rekurzivn� p�id�v� poduzly do TreeView.
        private void AddChildNodes(XmlNode xmlNode, TreeNode parentNode)
        {
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text) continue; // Textov� uzly se ignoruj�.
                var childTreeNode = CreateTreeNode(child);
                parentNode.Nodes.Add(childTreeNode);
                AddChildNodes(child, childTreeNode); // Rekurzivn� p�id�n� poduzl�.
            }

            // Pokud uzel nem� poduzly, nastav� se ikona na "soubor".
            if (parentNode.Nodes.Count == 0)
            {
                parentNode.ImageKey = "file.png";
                parentNode.SelectedImageKey = "file.png";
            }
        }

        // Vypr�zdn� TreeView a resetuje stav aplikace.
        private void ClearAll()
        {
            treeView.Nodes.Clear();
            fileInfoLabel.Text = elementInfoLabel.Text = string.Empty;
            currentFilePath = string.Empty;
        }

        // Zobraz� informace o vybran�m uzlu v TreeView.
        private void DisplayNodeInfo(TreeNode node)
        {
            if (node.Tag is XmlNode selectedNode)
            {
                if (selectedNode.NodeType == XmlNodeType.Element)
                {
                    var element = XElement.Parse(selectedNode.OuterXml); // Parsov�n� do XElement pro snadnou pr�ci.
                    elementInfoLabel.Text = $"Hloubka: {node.Level}\nPo�ad�: {node.Parent?.Nodes.IndexOf(node) + 1 ?? 0}\n" +
                                            $"Atributy: {string.Join(", ", element.Attributes().Select(a => $"{a.Name}=\"{a.Value}\""))}\nText: {element.Value.Trim()}";
                }
                else if (selectedNode.NodeType == XmlNodeType.Text)
                {
                    elementInfoLabel.Text = $"Hloubka: {node.Level}\nPo�ad�: {node.Parent?.Nodes.IndexOf(node) + 1 ?? 0}\nText: {selectedNode.Value.Trim()}";
                }
            }
        }

        // Zobraz� informace o na�ten�m XML souboru.
        private void DisplayFileInfo()
        {
            var root = XElement.Load(currentFilePath);
            fileInfoLabel.Text = $"Soubor: {Path.GetFileName(currentFilePath)}\n" +
                                 $"Maxim�ln� hloubka: {root.Descendants().Max(e => e.Ancestors().Count())}\n" +
                                 $"Maxim�ln� po�et potomk�: {root.Descendants().Max(e => e.Elements().Count())}\n" +
                                 $"Minim�ln� po�et atribut�: {root.DescendantsAndSelf().Min(e => e.Attributes().Count())}\n" +
                                 $"Maxim�ln� po�et atribut�: {root.DescendantsAndSelf().Max(e => e.Attributes().Count())}";
        }
    }
}