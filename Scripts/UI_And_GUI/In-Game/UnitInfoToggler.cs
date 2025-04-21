using Godot;
using System;
using System.Linq;
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

        private float buttonHoldTimer = 0;
        private float buttonHoldTimeToCancelBuyAndShowInfoInstead = 2f;
        private bool needToCheckForHold = false;
        private bool showInfoInstead = false;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
            //unit information should be loaded from a data file containing all the units their stats and costs that should be set here.

            InitializeUnitInfo();
        }

        private void InitializeUnitInfo()
        {
            //dont need to be async anymore
            //await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

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

                        int newFontSize = 24 + ((7 - unitName.Length) * 2);
                        int scaleMultiplier = 2; //make the text twice as big, but scale label down to make text clearer.

                        Vector2 originalLabelSize = new Vector2(105, 60); //this is the correct size for the name section

                        unitNameLabel.LabelSettings.FontSize = (newFontSize * scaleMultiplier);
                        unitNameLabel.Scale = new Vector2 (unitNameLabel.Scale.X / scaleMultiplier, unitNameLabel.Scale.Y / scaleMultiplier);
                        unitNameLabel.Size = originalLabelSize * scaleMultiplier;

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
                            
                            //int scaleMultiplier = 2; //make the text twice as big, but scale label down to make text clearer.

                            //Vector2 originalLabelSize = new Vector2(28, 23); //this is the correct size for the name section

                            //unitCostLabel.HorizontalAlignment = HorizontalAlignment.Center;
                            //unitCostLabel.VerticalAlignment = VerticalAlignment.Center;

                            //unitCostLabel.LabelSettings.FontSize = (16 * scaleMultiplier);
                            //unitCostLabel.Scale = new Vector2(unitCostLabel.Scale.X / scaleMultiplier, unitCostLabel.Scale.Y / scaleMultiplier);
                            //unitCostLabel.Size = originalLabelSize * scaleMultiplier;

                            //GD.Print("To font size: " + unitCostLabel.LabelSettings.FontSize);

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
                    //Set the unit description through the actual unit info file.

                    unitDescriptionLabel = description;
                    unitDescriptionLabel.Text = unitDescription.ToString();
                }
            }
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
            /*if (needToCheckForHold)
            {
                GD.Print("Current Hold time: " + buttonHoldTimer);
                if (Input.IsMouseButtonPressed(MouseButton.Left))
                {
                    if (buttonHoldTimer >= buttonHoldTimeToCancelBuyAndShowInfoInstead)
                    {
                        if (!showInfoInstead)
                        {
                            GD.Print("We show info instead");
                            showInfoInstead = true;
                            ShowUnitInfoOnHover();
                        }
                    }

                   
                    buttonHoldTimer += (float)delta;
                }
                else
                {
                    GD.Print("Released");

                    if(showInfoInstead) HideUnitInfoOnLoseHover();

                    //if we released it earlier we process the buy
                    if(buttonHoldTimer < buttonHoldTimeToCancelBuyAndShowInfoInstead)
                    {
                        CheckForMobileControlsAndProcessBuyUnit(false);
                    }

                    showInfoInstead = false;
                    needToCheckForHold = false;
                    buttonHoldTimer = 0;
                }
            }*/
		}

        private void LoadUnitValues()
        {
            switch (thisUnitAge)
            {
                case Enums.Ages.AGE_01:
                    switch (thisUnitType)
                    {
                        case Enums.UnitTypes.Warrior:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultWarrior = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Warrior.ToString())).FirstOrDefault().Value;

                                unitCost = defaultWarrior.unitCost;
                                unitHealth = defaultWarrior.unitHealth;
                                unitArmour = defaultWarrior.unitArmour;
                                unitAttack = defaultWarrior.unitAttack;

                                unitDescription = defaultWarrior.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Assassin:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultAssassin = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Assassin.ToString())).FirstOrDefault().Value;

                                unitCost = defaultAssassin.unitCost;
                                unitHealth = defaultAssassin.unitHealth;
                                unitArmour = defaultAssassin.unitArmour;
                                unitAttack = defaultAssassin.unitAttack;

                                unitDescription = defaultAssassin.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Enforcer:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultEnforcer = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Enforcer.ToString())).FirstOrDefault().Value;

                                unitCost = defaultEnforcer.unitCost;
                                unitHealth = defaultEnforcer.unitHealth;
                                unitArmour = defaultEnforcer.unitArmour;
                                unitAttack = defaultEnforcer.unitAttack;

                                unitDescription = defaultEnforcer.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Tank:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultTank = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Tank.ToString())).FirstOrDefault().Value;

                                unitCost = defaultTank.unitCost;
                                unitHealth = defaultTank.unitHealth;
                                unitArmour = defaultTank.unitArmour;
                                unitAttack = defaultTank.unitAttack;

                                unitDescription = defaultTank.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Battlemage:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultBattlemage = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Battlemage.ToString())).FirstOrDefault().Value;

                                unitCost = defaultBattlemage.unitCost;
                                unitHealth = defaultBattlemage.unitHealth;
                                unitArmour = defaultBattlemage.unitArmour;
                                unitAttack = defaultBattlemage.unitAttack;

                                unitDescription = defaultBattlemage.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Mass_Healer:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultMassHealer = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Mass_Healer.ToString())).FirstOrDefault().Value;

                                unitCost = defaultMassHealer.unitCost;
                                unitHealth = defaultMassHealer.unitHealth;
                                unitArmour = defaultMassHealer.unitArmour;
                                unitAttack = defaultMassHealer.unitAttack;

                                unitDescription = defaultMassHealer.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Ranger:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultRanger = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Ranger.ToString())).FirstOrDefault().Value;

                                unitCost = defaultRanger.unitCost;
                                unitHealth = defaultRanger.unitHealth;
                                unitArmour = defaultRanger.unitArmour;
                                unitAttack = defaultRanger.unitAttack;

                                unitDescription = defaultRanger.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Archdruid:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age01_ArchdruidSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_ArchdruidSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_ArchdruidSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_ArchdruidSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_ArchdruidSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_ArchdruidSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultArchdruid = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Archdruid.ToString())).FirstOrDefault().Value;

                                unitCost = defaultArchdruid.unitCost;
                                unitHealth = defaultArchdruid.unitHealth;
                                unitArmour = defaultArchdruid.unitArmour;
                                unitAttack = defaultArchdruid.unitAttack;

                                unitDescription = defaultArchdruid.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Shaman:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age01_ShamanSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_ShamanSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_ShamanSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_ShamanSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_ShamanSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_ShamanSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultShaman = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Shaman.ToString())).FirstOrDefault().Value;

                                unitCost = defaultShaman.unitCost;
                                unitHealth = defaultShaman.unitHealth;
                                unitArmour = defaultShaman.unitArmour;
                                unitAttack = defaultShaman.unitAttack;

                                unitDescription = defaultShaman.unitDescription;
                            }
                            break;
                        default:
                            GD.PrintErr("UNIT TYPE NOT IMPLEMENTED: UnitInfoToggler.cs");
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultWarrior = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Warrior.ToString())).FirstOrDefault().Value;

                                unitCost = defaultWarrior.unitCost;
                                unitHealth = defaultWarrior.unitHealth;
                                unitArmour = defaultWarrior.unitArmour;
                                unitAttack = defaultWarrior.unitAttack;

                                unitDescription = defaultWarrior.unitDescription;
                            }
                            break;
                    }
                    break;
                case Enums.Ages.AGE_02:
                    switch (thisUnitType)
                    {
                        case Enums.UnitTypes.Warrior:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultWarrior = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Warrior.ToString())).FirstOrDefault().Value;

                                unitCost = defaultWarrior.unitCost;
                                unitHealth = defaultWarrior.unitHealth;
                                unitArmour = defaultWarrior.unitArmour;
                                unitAttack = defaultWarrior.unitAttack;

                                unitDescription = defaultWarrior.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Assassin:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultAssassin = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Assassin.ToString())).FirstOrDefault().Value;

                                unitCost = defaultAssassin.unitCost;
                                unitHealth = defaultAssassin.unitHealth;
                                unitArmour = defaultAssassin.unitArmour;
                                unitAttack = defaultAssassin.unitAttack;

                                unitDescription = defaultAssassin.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Enforcer:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultEnforcer = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Enforcer.ToString())).FirstOrDefault().Value;

                                unitCost = defaultEnforcer.unitCost;
                                unitHealth = defaultEnforcer.unitHealth;
                                unitArmour = defaultEnforcer.unitArmour;
                                unitAttack = defaultEnforcer.unitAttack;

                                unitDescription = defaultEnforcer.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Tank:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultTank = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Tank.ToString())).FirstOrDefault().Value;

                                unitCost = defaultTank.unitCost;
                                unitHealth = defaultTank.unitHealth;
                                unitArmour = defaultTank.unitArmour;
                                unitAttack = defaultTank.unitAttack;

                                unitDescription = defaultTank.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Battlemage:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultBattlemage = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Battlemage.ToString())).FirstOrDefault().Value;

                                unitCost = defaultBattlemage.unitCost;
                                unitHealth = defaultBattlemage.unitHealth;
                                unitArmour = defaultBattlemage.unitArmour;
                                unitAttack = defaultBattlemage.unitAttack;

                                unitDescription = defaultBattlemage.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Mass_Healer:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultMassHealer = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Mass_Healer.ToString())).FirstOrDefault().Value;

                                unitCost = defaultMassHealer.unitCost;
                                unitHealth = defaultMassHealer.unitHealth;
                                unitArmour = defaultMassHealer.unitArmour;
                                unitAttack = defaultMassHealer.unitAttack;

                                unitDescription = defaultMassHealer.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Ranger:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultRanger = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Ranger.ToString())).FirstOrDefault().Value;

                                unitCost = defaultRanger.unitCost;
                                unitHealth = defaultRanger.unitHealth;
                                unitArmour = defaultRanger.unitArmour;
                                unitAttack = defaultRanger.unitAttack;

                                unitDescription = defaultRanger.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Archdruid:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age02_ArchdruidSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_ArchdruidSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_ArchdruidSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_ArchdruidSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_ArchdruidSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_ArchdruidSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultArchdruid = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Archdruid.ToString())).FirstOrDefault().Value;

                                unitCost = defaultArchdruid.unitCost;
                                unitHealth = defaultArchdruid.unitHealth;
                                unitArmour = defaultArchdruid.unitArmour;
                                unitAttack = defaultArchdruid.unitAttack;

                                unitDescription = defaultArchdruid.unitDescription;
                            }
                            break;
                        case Enums.UnitTypes.Shaman:
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age02_ShamanSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_ShamanSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_ShamanSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_ShamanSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_ShamanSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_ShamanSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultShaman = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Shaman.ToString())).FirstOrDefault().Value;

                                unitCost = defaultShaman.unitCost;
                                unitHealth = defaultShaman.unitHealth;
                                unitArmour = defaultShaman.unitArmour;
                                unitAttack = defaultShaman.unitAttack;

                                unitDescription = defaultShaman.unitDescription;
                            }
                            break;
                        default:
                            GD.PrintErr("UNIT TYPE NOT IMPLEMENTED: UnitInfoToggler.cs");
                            if (GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.useCustomVariables)
                            {
                                unitCost = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitCost;
                                unitHealth = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitHealth;
                                unitArmour = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitArmour;
                                unitAttack = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitAttack;

                                unitDescription = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig.unitDescription;
                            }
                            else
                            {
                                UnitSettingsConfig defaultWarrior = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Warrior.ToString())).FirstOrDefault().Value;

                                unitCost = defaultWarrior.unitCost;
                                unitHealth = defaultWarrior.unitHealth;
                                unitArmour = defaultWarrior.unitArmour;
                                unitAttack = defaultWarrior.unitAttack;

                                unitDescription = defaultWarrior.unitDescription;
                            }
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void CheckForMobileControlsAndProcessBuyUnit(bool needToCheck)
        {
            /*if (needToCheck)
            {
                GD.Print("NeedToCheck is true, " + needToCheckForHold);
                return;
            }*/

            if (buttonPressed) return;

            buttonPressed = true;

            bool success = GameManager.Instance.inGameHUDManager.BuyUnitButtonClicked(thisUnitType, unitCost);

            if (!success)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                buttonPressed = false;
            }
            else
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
                GameManager.Instance.inGameHUDManager.RefreshUnitShopSpecificButton(this.GetInstanceId());
            }
        }

        public void BuyUnitButtonPressed()
        {
            if (GameManager.Instance.gameIsPaused || GameManager.Instance.gameIsFinished) return;

            /*needToCheckForHold = true;
            GD.Print("Set the variable to true: " + needToCheckForHold);*/

            CheckForMobileControlsAndProcessBuyUnit(true);
        }

        public void ShowUnitInfoOnHover()
        {
            //we dont want to collapse this if we still have focus.
            if (GameSettingsLoader.Instance.useHighlightFocusMode)
            {
                if (!HasFocus()) return;
            }

            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonHoverAudioClip);

            foreach (var child in this.GetChildren())
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

        public void ShowUnitInfoOnFocus()
        {
            if (!GameSettingsLoader.Instance.useHighlightFocusMode) return;

            ShowUnitInfoOnHover();
        }

        public void HideUnitInfoOnLoseFocus()
        {
            if (!GameSettingsLoader.Instance.useHighlightFocusMode) return;

            HideUnitInfoOnLoseHover();
        }

        public void HideUnitInfoOnLoseHover()
        {
            //we dont want to collapse this if we still have focus.
            if (GameSettingsLoader.Instance.useHighlightFocusMode)
            {
                if (HasFocus()) return;
            }

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
