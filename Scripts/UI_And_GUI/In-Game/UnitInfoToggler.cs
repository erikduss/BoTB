using Godot;
using System;
using static Erikduss.Enums;

namespace Erikduss
{
	public partial class UnitInfoToggler : TextureButton
	{
        [Export] public Enums.UnitTypes thisUnitType = Enums.UnitTypes.Warrior;
        [Export] public Enums.Ages thisUnitAge = Enums.Ages.AGE_01;
        public int unitCost = 10;
        public int unitHealth = 20;
        public int unitArmour = 20;
        public int unitAttack = 10;

        public string unitDescription = "nothing";

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

            InitializeUnitInfo();
        }

        async void InitializeUnitInfo()
        {
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

            LoadUnitValues();

            Node parentNode = this.GetChild(0).GetChild(0);

            foreach (Node labelNode in parentNode.GetChildren())
            {
                if (labelNode is Label)
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

                foreach (Node labelInComp in labelNode.GetChildren())
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

        private void LoadUnitValues()
        {
            switch (thisUnitAge)
            {
                case Enums.Ages.AGE_01:
                    switch (thisUnitType)
                    {
                        case Enums.UnitTypes.Warrior:
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Asssassin:
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Enforcer:
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Tank:
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Battlemage:
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Mass_Healer:
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Ranger:
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.unitDescription;
                            break;
                    }
                    break;
                case Enums.Ages.AGE_02:
                    switch (thisUnitType)
                    {
                        case Enums.UnitTypes.Warrior:
                            unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitCost;
                            unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitHealth;
                            unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitArmour;
                            unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitAttack;

                            unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Asssassin:
                            unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.unitCost;
                            unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.unitHealth;
                            unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.unitArmour;
                            unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.unitAttack;

                            unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Enforcer:
                            unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.unitCost;
                            unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.unitHealth;
                            unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.unitArmour;
                            unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.unitAttack;

                            unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Tank:
                            unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.unitCost;
                            unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.unitHealth;
                            unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.unitArmour;
                            unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.unitAttack;

                            unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Battlemage:
                            unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.unitCost;
                            unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.unitHealth;
                            unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.unitArmour;
                            unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.unitAttack;

                            unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Mass_Healer:
                            unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.unitCost;
                            unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.unitHealth;
                            unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.unitArmour;
                            unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.unitAttack;

                            unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.unitDescription;
                            break;
                        case Enums.UnitTypes.Ranger:
                            unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.unitCost;
                            unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.unitHealth;
                            unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.unitArmour;
                            unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.unitAttack;

                            unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.unitDescription;
                            break;
                    }
                    break;
                default:
                    break;
            }
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
