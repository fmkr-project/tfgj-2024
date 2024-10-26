using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace UI
{
	public struct Date
	{
		public int Month;
		public int Day;
		
		
	}
	
	// This comes from https://qiita.com/devmynote/items/015483085e1d6e1a209b
	public class JCalendar {
		static string[] sEra    = // 元号
			{ "", "明治", "大正", "昭和", "平成", "令和" };
		static string[] sRokuyo = // 六曜
			{ "大安", "赤口", "先勝", "友引", "先負", "仏滅" };
		static string[] sKanshi = // 天干
			{ "", "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
		static string[] sChishi = // 地支
			{ "", "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
	
		public static Date Conv()
		{
			var newDate = DateTime.Now;
			
			return printJapaneseLunisolarCalendar(newDate);
			
		}
		
		static Date printJapaneseLunisolarCalendar(DateTime newDate)
		{
			JapaneseLunisolarCalendar jlc = new JapaneseLunisolarCalendar();
			int era   = jlc.GetEra(newDate);
			int year  = jlc.GetYear(newDate);
			int month = jlc.GetMonth(newDate);
			int day   = jlc.GetDayOfMonth(newDate);
		
			//閏月を取得
			string sLeap = "";
			if ( year > 0 ){
				int leapMonth = jlc.GetLeapMonth(year, era);
				if ( month == leapMonth ){
					sLeap = "（閏月）";
				}
				//閏月含む場合の月を補正
				if ( (leapMonth > 0) && (month >= leapMonth) ){
					month = month - 1;	//旧暦月の補正
				}
			}
			// 干支（天干、地支）
			int sy = jlc.GetSexagenaryYear(newDate);
			int tk = jlc.GetCelestialStem(sy);
			int ts = jlc.GetTerrestrialBranch(sy);
		
			// 六曜(大安・赤口・先勝・友引・先負・仏滅)
			// (月 + 日) % 6 の余り
			int rokuyo = (month + day) % 6;

			var j = new Date();
			j.Month = month;
			j.Day = day;
			return j;
		}
	}

	public class SetDate : MonoBehaviour
	{
		// Does only one thing: set the date.
		void Start()
		{
			var dateUGUI = transform.Find("Canvas/Date").GetComponent<TextMeshProUGUI>();
			dateUGUI.text = "";
			var months = new List<String> { "睦月", "如月", "弥生", "卯月", "皐月", "水無月", "文月", "葉月", "長月", "神無月", "霜月", "師走" };
			var days = new List<String> { "日", "月", "火", "水", "木", "金", "土" };
			
			var date = JCalendar.Conv();
			var temp = $"{months[date.Month - 1]}の";
			foreach (var c in temp)
				dateUGUI.text += $"{c}\n";
			dateUGUI.text += $"{date.Day}\n";
			dateUGUI.text += $"({days[(int) DateTime.Now.DayOfWeek]})";
		}
	}
}