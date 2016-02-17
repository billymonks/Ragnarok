using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.eventscript;
using System.Xml.Linq;

namespace wickedcrush.manager.eventscript
{
	public class EventManager
	{
        Dictionary<int, EventScript> scripts;

        public EventManager(GameBase game)
        {
            scripts = new Dictionary<int, EventScript>();

            LoadScripts();
        }
        public EventScript GetEvent(int id)
        {
            return scripts[id];
        }
        
        private void LoadScripts()
        {
            scripts.Clear();

            String path = "Content/scripts/eventscripts.xml";
            XDocument doc = XDocument.Load(path);
            XElement rootElement = new XElement(doc.Element("events"));

            int id;

            foreach (XElement e in rootElement.Elements("event"))
            {
                id = int.Parse(e.Attribute("id").Value);
                scripts.Add(id, new EventScript(GetChildElements(e, null)));
            }
        }

        private List<EventNode> GetChildElements(XElement currentElement, EventNode parent)
        {
            String type;
            List<EventNode> nodeList = new List<EventNode>();

            EventNode newNode;

            foreach (XElement e in currentElement.Elements("node"))
            {
                type = e.Attribute("type").Value;

                switch (type)
                {
                    case "dialog":
                        newNode = new DialogNode(e.Value, parent);
                        nodeList.Add(newNode);
                        break;
                    case "question":
                        newNode = GetQuestionNode(e, parent);
                        nodeList.Add(newNode);
                        break;
                    case "answer":
                        newNode = GetAnswerNode(e, parent);
                        nodeList.Add(newNode);
                        break;
                    case "checkint":
                        newNode = GetCheckIntNode(e, parent);
                        nodeList.Add(newNode);
                        break;
                    case "intval":
                        newNode = GetIntValNode(e, parent);
                        nodeList.Add(newNode);
                        break;
                    case "setint":
                        newNode = GetSetIntNode(e, parent);
                        nodeList.Add(newNode);
                        break;
                    case "itemget":
                        newNode = GetItemGetNode(e, parent);
                        nodeList.Add(newNode);
                        break;
                    case "equip":
                        newNode = GetEquipNode(e, parent);
                        nodeList.Add(newNode);
                        break;
                }
            }

            for (int i = 0; i < nodeList.Count - 1; i++)
            {
                nodeList[i].next = nodeList[i + 1];
            }

            return nodeList;
        }

        private QuestionNode GetQuestionNode(XElement questionElement, EventNode parent)
        {
            QuestionNode question = new QuestionNode(questionElement.Attribute("text").Value, questionElement.Attribute("key").Value, parent);
            
            question.SetChildren(GetChildElements(questionElement, question));

            return question;

        }

        private AnswerNode GetAnswerNode(XElement answerElement, EventNode parent)
        {
            AnswerNode answerNode = new AnswerNode(answerElement.Attribute("text").Value, int.Parse(answerElement.Attribute("val").Value), parent);

            //answerNode.SetChildren(GetChildElements(answerElement, answerNode));

            return answerNode;
        }

        private CheckIntNode GetCheckIntNode(XElement element, EventNode parent)
        {
            CheckIntNode node = new CheckIntNode(element.Attribute("key").Value, parent);

            node.SetChildren(GetChildElements(element, node));

            return node;
        }

        private SetIntNode GetSetIntNode(XElement element, EventNode parent)
        {
            SetIntNode node = new SetIntNode(element.Attribute("key").Value, int.Parse(element.Value), parent);

            node.SetChildren(GetChildElements(element, node));

            return node;
        }

        private EquipNode GetEquipNode(XElement element, EventNode parent)
        {
            EquipNode node = new EquipNode(parent);

            return node;
        }

        private ItemGetNode GetItemGetNode(XElement element, EventNode parent)
        {
            ItemGetNode node = new ItemGetNode(element.Attribute("item").Value, parent);

            return node;
        }

        private IntValNode GetIntValNode(XElement element, EventNode parent)
        {
            IntValNode node = new IntValNode(int.Parse(element.Attribute("val").Value), parent);

            node.SetChildren(GetChildElements(element, node));

            return node;
        }
	}
}
