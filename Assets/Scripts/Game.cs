using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;


public class Game : MonoBehaviour
{
	[Header("Текст, отвечающий за отображение денег")]
	public Text scoreText;
	[Header("Магазин")]
	public List<Item> shopItems = new List<Item>();
	[Header("Текст на кнопках товаров")]
	public Text [] shopItemsText;
	[Header("Кнопки товаров")]
	public Button[] shopBttns;
	private int bonus = 1;
	[Header("Панель магазина")]
	public GameObject shopPan;
	
	private int score; // Игровая валюта
	private int scoreIncrease = 1; //Бонус при клике
	
	private void Start()
	{
		updateCosts(); //Обновить текст с ценами
		StartCoroutine(BonusPerSec());// Запустить просчёт бонуса в секунду
	}
	
	public void Update()
	{
		scoreText.text = score + "$";//Отображаем деньги
	}
	
	
	
	public void BuyBttn(int index)//Метод при нажатии на кнопку покупки товара(индекс товара)
	{
		int cost = shopItems[index].cost * shopItems[shopItems[index].itemIndex].bonusCounter;//Расчитываем цену в зависимости от кол-ва рабочих(к примеру)
		if (shopItems[index].itsBonus && score >= cost) // Если товар нажатой кнопки - это бонус, и денег >= цены(е)
		{
			if (cost > 0)// Если цена больше чем 0, то
			{
				score -= cost; // Вычитаем цену
				StartCoroutine(BonusTimer(shopItems[index].timeOfBonus, index)); //запускаем бонусный таймер
			}
			else print("Нечего улучшать!"); // иначе выводим в консоль текст
		}
		else if (score >= shopItems[index].cost) // Иначе, если товар нажатой кнопки - это не бонус, и денег < цена
		{
			if (shopItems[index].itsItemPerSec) shopItems[index].bonusCounter++; //Если нанимаем рабочего (к примеру), то прибавляем кол-во рабочих
			else scoreIncrease += shopItems[index].bonusIncrease; //иначе бонусу при клике добавляем бонус товара
			score -= shopItems[index].cost; // вычитаем цену из денег
			if (shopItems[index].needCostMultiplier) shopItems[index].cost *= shopItems[index].costMultiplier;// Если товару нужно узнать цену, то умножаем на множитель
			shopItems[index].levelOfItem++;// Поднимаем уровень предмета на 1
		}
		else print("Не хватает денег");//Иначе, если две проверки равны false, то выводим в консоль текст
		updateCosts();//Обновить текст с ценами
	}
	
	
	
	public void updateCosts()//Метод для обновления текста с ценами
	{
		for (int i = 0; i < shopItems.Count; i++)//Цикл выполняется, пока переменная i < кол-ва товаров
		{
			if (shopItems[i].itsBonus)// Если товар является бонусом, то:
			{
				int cost = shopItems[i].cost * shopItems[shopItems[i].itemIndex].bonusCounter;// Расчитываем цену в зависимости от кол-ва рабочих
				shopItemsText[i].text = shopItems[i].name + "\n" + cost + "$"; // обновляем текст кнопки с расчитанной ценой
			}
			else shopItemsText[i].text = shopItems[i].name + "\n" + shopItems[i].cost + "$"; //иначе если товар не является бонусом, то обновляем текст кнопки с обычной ценой 
		}
	}
	
	
	
	IEnumerator BonusPerSec()// бонус в секунду
	{
		while (true)//бесконечный цикл
		{
			for (int i = 0; i < shopItems.Count; i++) score += (shopItems[i].bonusCounter * shopItems[i].bonusPerSec);//Добавляем к игровой валюте бонус рабочих
			yield return new WaitForSeconds(1);// делаем задержку в 1 секунду
		}
	}
	
	
	IEnumerator BonusTimer(float time, int index)// бонусный таймер (длительность бонуса (в сек.), индекс товара) 
	{
		shopBttns[index].interactable = false;//Выключаем кликабельность кнопки бонуса
		shopItems[shopItems[index].itemIndex].bonusPerSec *= 2;//Удваиваем бонус рабочих в секунду
		yield return new WaitForSeconds(time);//Делаем задержку на столько секунд, сколько указали в параметре
		shopItems[shopItems[index].itemIndex].bonusPerSec /= 2;//Возвращаем бонус в нормальное состояние
		shopBttns[index].interactable = true;//Включаем кликабельность кнопки
	}
	
	public void shopPan_ShowAndHide()
	{
		shopPan.SetActive(!shopPan.activeSelf);
	}
	
    public void OnClick ()
	{
		score += bonus;// к игровой валюте прибавляем бонус при клике
	}
	
}
[Serializable]
public class Item // Класс товара
{
	[Tooltip("Название используется на кнопках")]
	public string name;
	[Tooltip("Цена товара")]
	public int cost;
	[Tooltip("Бонус, который добавляется к бонусу при клике")]
	public int bonusIncrease;
	[HideInInspector]
	public int levelOfItem; //Уровень товара
	[Space]
	[Tooltip("Нужен ли множитель для цены?")]
	public bool needCostMultiplier;
	[Tooltip("Множитель для цены")]
	public int costMultiplier;
	[Space]
	[Tooltip("Этот товар дает бонус в секунду?")]
	public bool itsItemPerSec;
	[Tooltip("Бонус, который дается в секунду")]
	public int bonusPerSec;
	[HideInInspector]
	public int bonusCounter;
	[Space]
	[Tooltip("Это временный бонус?")]
	public bool itsBonus;
	[Tooltip("Множитель товара, который управляется бонусом (Умножается переменная bonusPerSec)")]
	public int itemMultiplier;
	[Tooltip("Индекс товара, который будет управляться бонусом (умножается переменная bonusPerSec этого товара)")]
	public int itemIndex;
	[Tooltip("Длительность бонуса")]
	public float timeOfBonus;
}
