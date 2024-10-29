using Godot;
using System;

namespace Erikduss
{
	public partial class UnitInfoToggler : TextureButton
	{
        [Export] public Enums.UnitTypes thisUnitType = Enums.UnitTypes.Warrior;
        [Export] public int unitCost = 10;
        [Export] public int unitHealth = 20;
        [Export] public int unitArmour = 20;
        [Export] public int unitAttack = 10;

        [Export] public string unitDescription = "nothing";

        private Label unitNameLabel;
        private Label unitDescriptionLabel;

        private Label unitCostLabel;
        private Label unitHealthLabel;
        private Label unitArmourLabel;
        private Label unitAttackLabel;

        private bool buttonPressed = false;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
            //unit information should be loaded from a data file containing all the units their stats and costs that should be set here.

            Node parentNode = this.GetChild(0).GetChild(0);

            foreach (Node labelNode in parentNode.GetChildren())
            {
                if(labelNode is Label)
                {
                    //set the unit name to the set ENUM
                    Label labelComponent = labelNode.GetNode<Label>(labelNode.GetPath());

                    if (labelComponent.Name == "UnitName")
                    {
                        unitNameLabel = labelComponent;

                        //filter the names of the units that have multiple worlds and replace it with a space.
                        string unitName = thisUnitType.ToString();
                        unitName = unitName.Replace("_", " ");

                        unitNameLabel.LabelSettings.FontSize = 24 + ((8 - unitName.Length) * 3);

                        unitNameLabel.Text = unitName;
                    }
                }

                foreach(Node labelInComp in labelNode.GetChildren())
                {
                    if (labelInComp is Label)
                    {
                        Label labelComponent = labelInComp.GetNode<Label>(labelInComp.GetPath());

                        if (labelComponent.Name == "UnitCost")
                        {
                            unitCostLabel = labelComponent;
                            unitCostLabel.Text = unitCost.ToString();
                        }
                        else if (labelComponent.Name == "HealthValue")
                        {
                            unitHealthLabel = labelComponent;
                            unitHealthLabel.Text = unitHealth.ToString();
                        }
                        else if (labelComponent.Name == "ArmorValue")
                        {
                            unitArmourLabel = labelComponent;
                            unitArmourLabel.Text = unitArmour.ToString();
                        }
                        else if (labelComponent.Name == "AttackValue")
                        {
                            unitAttackLabel = labelComponent;
                            unitAttackLabel.Text = unitAttack.ToString();
                        }
                    }
                }
            }

            Node descriptionParentNode = GetChild(0).GetChild(1);

            foreach (Node child in descriptionParentNode.GetChildren())
            {
                if (child is Label)
                {
                    Label description = child.GetNode<Label>(child.GetPath());

                    GD.Print(description.Name);
                    //Set the unit description through the actual unit info file.

                    unitDescriptionLabel = description;
                    unitDescriptionLabel.Text = unitDescription.ToString();
                }
            }
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void BuyUnitButtonPressed()
        {
            if (buttonPressed) return;

            buttonPressed = true;

            bool success = GameManager.Instance.inGameHUDManager.BuyUnitButtonClicked(thisUnitType, unitCost);

            if(!success) buttonPressed = false;

            else
            {
                GameManager.Instance.inGameHUDManager.RefreshUnitShopSpecificButton(this.GetInstanceId());
            }
        }

        public void ShowUnitInfoOnHover()
        {
            foreach(var child in this.GetChildren())
            {
                if (child is TextureRect)
                {
                    TextureRect unitInfoPanel = child as TextureRect;

                    if (unitInfoPanel != null)
                    {
                        unitInfoPanel.Visible = true;
                    }

                    continue;
                }
            }
        }

        public void HideUnitInfoOnLoseHover()
        {
            foreach (var child in this.GetChildren())
            {
                if (child is TextureRect)
                {
                    TextureRect unitInfoPanel = child as TextureRect;

                    if (unitInfoPanel != null)
                    {
                        unitInfoPanel.Visible = false;
                    }

                    continue;
                }
            }
        }
    }
}
