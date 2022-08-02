using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerOrderGenerator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI customerOrderDisplayText;
    [SerializeField] private TextMeshProUGUI testingText;

    private HashSet<MakeBurger.BurgerContent> _answerSet;

    private void Start()
    {
        _answerSet = new HashSet<MakeBurger.BurgerContent>();
        GenerateCustomerOrder();
    }

    private void GenerateCustomerOrder()
    {
        Recipe recipe = new Recipe1();
        customerOrderDisplayText.text = recipe.GenerateCustomerOrder(out _answerSet);
        testingText.text = recipe.GetRecipe();
    }
}

abstract class Recipe
{
    protected readonly string[] Patties = {"Beef", "Chicken", "Fish"};
    protected readonly string[] Toppings = {"Cheese", "Cucumber", "Onion", "Lettuce", "Tomato"};
    protected readonly string[] Sauces = {"BBQ", "Chili", "Ketchup", "Mayo", "Mustard"};

    protected const string PinkColorCode = "#FF7979";
    protected const string BlueColorCode = "#83E1F5";
    protected const string YellowColorCode = "#FEF06F";
    protected const string GreenColorCode = "#BAFF90";
    protected const string OrangeColorCode = "#FCA846";

    public abstract string GetRecipe();
    public abstract string GenerateCustomerOrder(out HashSet<MakeBurger.BurgerContent> answerSet);

    protected MakeBurger.BurgerContent MapStringToBurgerContent(string content)
    {
        switch (content)
        {
            case "Beef":
                return MakeBurger.BurgerContent.BeefPatty;
            case "Chicken":
                return MakeBurger.BurgerContent.ChickenPatty;
            case "Fish":
                return MakeBurger.BurgerContent.FishPatty;
            case "Cheese":
                return MakeBurger.BurgerContent.Cheese;
            case "Cucumber":
                return MakeBurger.BurgerContent.Cucumber;
            case "Onion":
                return MakeBurger.BurgerContent.Onion;
            case "Lettuce":
                return MakeBurger.BurgerContent.Salad;
            case "Tomato":
                return MakeBurger.BurgerContent.Tomato;
            case "BBQ":
                return MakeBurger.BurgerContent.BbqSauce;
            case "Chili":
                return MakeBurger.BurgerContent.ChiliSauce;
            case "Ketchup":
                return MakeBurger.BurgerContent.KetchupSauce;
            case "Mayo":
                return MakeBurger.BurgerContent.MayoSauce;
            case "Mustard":
                return MakeBurger.BurgerContent.MustardSauce;
            default:
                throw new Exception();
        }
    }

    protected void RemoveAllPattiesFromSet(HashSet<MakeBurger.BurgerContent> hashSet)
    {
        hashSet.Remove(MakeBurger.BurgerContent.BeefPatty);
        hashSet.Remove(MakeBurger.BurgerContent.ChickenPatty);
        hashSet.Remove(MakeBurger.BurgerContent.FishPatty);
    }

    protected void RemoveAllToppingsFromSet(HashSet<MakeBurger.BurgerContent> hashSet)
    {
        hashSet.Remove(MakeBurger.BurgerContent.Cheese);
        hashSet.Remove(MakeBurger.BurgerContent.Cucumber);
        hashSet.Remove(MakeBurger.BurgerContent.Onion);
        hashSet.Remove(MakeBurger.BurgerContent.Salad);
        hashSet.Remove(MakeBurger.BurgerContent.Tomato);
    }

    protected void RemoveAllSaucesFromSet(HashSet<MakeBurger.BurgerContent> hashSet)
    {
        hashSet.Remove(MakeBurger.BurgerContent.BbqSauce);
        hashSet.Remove(MakeBurger.BurgerContent.ChiliSauce);
        hashSet.Remove(MakeBurger.BurgerContent.KetchupSauce);
        hashSet.Remove(MakeBurger.BurgerContent.MayoSauce);
        hashSet.Remove(MakeBurger.BurgerContent.MustardSauce);
    }
}

class Recipe1 : Recipe
{
    private readonly string _recipe =
        $"<color={PinkColorCode}>class</color> <color={BlueColorCode}>Burger</color> {{\n\t<color={PinkColorCode}>private</color> <color={BlueColorCode}>String</color> pattyType = <color={YellowColorCode}>\"Chicken\"</color>;\n\t<color={PinkColorCode}>private</color> <color={BlueColorCode}>String</color> sauceType = <color={YellowColorCode}>\"Chili\"</color>;\n\t<color={PinkColorCode}>private</color> <color={PinkColorCode}>boolean</color> hasLettuce;\n\n\t<color={PinkColorCode}>public</color> <color={GreenColorCode}>Burger</color>() {{\n\n\t}}\n\n\t<color={PinkColorCode}>public</color> <color={GreenColorCode}>Burger</color>(<color={BlueColorCode}>String</color> <color={OrangeColorCode}>pattyType</color>, <color={BlueColorCode}>String</color> <color={OrangeColorCode}>sauceType</color>, <color={PinkColorCode}>boolean</color> <color={OrangeColorCode}>hasLettuce</color>) {{\n\t\t<color={PinkColorCode}>this</color>.pattyType = <color={OrangeColorCode}>pattyType</color>;\n\t\t<color={PinkColorCode}>this</color>.sauceType = <color={OrangeColorCode}>sauceType</color>;\n\t\t<color={PinkColorCode}>this</color>.hasLettuce = <color={OrangeColorCode}>hasLettuce</color>;\n\t}}\n\n\t<color={PinkColorCode}>public</color> <color={PinkColorCode}>void</color> <color={GreenColorCode}>setPattyType</color>(<color={BlueColorCode}>String</color> <color={OrangeColorCode}>pattyType</color>) {{\n\t\t<color={PinkColorCode}>this</color>.pattyType = <color={OrangeColorCode}>pattyType</color>;\n\t}}\n\n\t<color={PinkColorCode}>public</color> <color={PinkColorCode}>void</color> <color={GreenColorCode}>setSauceType</color>(<color={BlueColorCode}>String</color> <color={OrangeColorCode}>sauceType</color>) {{\n\t\t<color={PinkColorCode}>this</color>.sauceType = <color={OrangeColorCode}>sauceType</color>;\n\t}}\n}}";

    public override string GetRecipe()
    {
        return _recipe;
    }

    public override string GenerateCustomerOrder(out HashSet<MakeBurger.BurgerContent> answerSet)
    {
        StringBuilder stringBuilder = new StringBuilder();
        HashSet<MakeBurger.BurgerContent> set = new HashSet<MakeBurger.BurgerContent>();

        // randomly pick constructor used
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
            {
                stringBuilder.Append(
                    $"<color={BlueColorCode}>Burger</color> burger <color={PinkColorCode}>= " +
                    $"new</color> <color={GreenColorCode}>Burger</color> ();\n");

                set.Add(MakeBurger.BurgerContent.ChickenPatty);
                set.Add(MakeBurger.BurgerContent.ChiliSauce);

                break;
            }
            case 1:
            {
                string patty = Patties[Random.Range(0, Patties.Length)];
                string sauce = Sauces[Random.Range(0, Sauces.Length)];

                stringBuilder.Append(
                    $"<color={BlueColorCode}>Burger</color> burger <color={PinkColorCode}>= " +
                    $"new</color> <color={GreenColorCode}>Burger</color> " +
                    $"(<color={YellowColorCode}>\"{patty}\"</color>, <color={YellowColorCode}>\"{sauce}\"</color>, " +
                    $"<color={PinkColorCode}>true</color>);\n");

                set.Add(MapStringToBurgerContent(patty));
                set.Add(MapStringToBurgerContent(sauce));
                set.Add(MakeBurger.BurgerContent.Salad);

                break;
            }
            case 2:
            {
                string patty = Patties[Random.Range(0, Patties.Length)];
                string sauce = Sauces[Random.Range(0, Sauces.Length)];

                stringBuilder.Append(
                    $"<color={BlueColorCode}>Burger</color> burger <color={PinkColorCode}>= " +
                    $"new</color> <color={GreenColorCode}>Burger</color> " +
                    $"(<color={YellowColorCode}>\"{patty}\"</color>, <color={YellowColorCode}>\"{sauce}\"</color>, " +
                    $"<color={PinkColorCode}>false</color>);\n");

                set.Add(MapStringToBurgerContent(patty));
                set.Add(MapStringToBurgerContent(sauce));

                break;
            }
        }

        // randomly pick add-on setters used
        rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0:
            {
                // do nothing
                break;
            }
            case 1:
            {
                // add patty type setter
                string patty = Patties[Random.Range(0, Patties.Length)];

                stringBuilder.Append(
                    $"burger.<color={GreenColorCode}>setPattyType</color>(<color={YellowColorCode}>\"{patty}\"</color>);\n");

                // update the patty used
                RemoveAllPattiesFromSet(set);
                set.Add(MapStringToBurgerContent(patty));

                break;
            }
            case 2:
            {
                // add sauce type setter
                string sauce = Sauces[Random.Range(0, Sauces.Length)];

                stringBuilder.Append(
                    $"burger.<color={GreenColorCode}>setSauceType</color>(<color={YellowColorCode}>\"{sauce}\"</color>);\n");

                // update the sauce used
                RemoveAllSaucesFromSet(set);
                set.Add(MapStringToBurgerContent(sauce));

                break;
            }
            case 3:
            {
                // add patty and sauce type setters
                string patty = Patties[Random.Range(0, Patties.Length)];
                string sauce = Sauces[Random.Range(0, Sauces.Length)];

                stringBuilder.Append(
                    $"burger.<color={GreenColorCode}>setPattyType</color>(<color={YellowColorCode}>\"{patty}\"</color>);\n");
                stringBuilder.Append(
                    $"burger.<color={GreenColorCode}>setSauceType</color>(<color={YellowColorCode}>\"{sauce}\"</color>);\n");

                // update the patty and sauce used
                RemoveAllPattiesFromSet(set);
                set.Add(MapStringToBurgerContent(patty));
                RemoveAllSaucesFromSet(set);
                set.Add(MapStringToBurgerContent(sauce));

                break;
            }
        }

        answerSet = set;
        return stringBuilder.ToString();
    }
}