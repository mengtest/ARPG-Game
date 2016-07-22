﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UGameTools;

namespace Windows
{
	[UIResources ("GUIBattle")]
	public class GUIBattle : UUIWindow
	{

		protected override void OnCreate ()
		{
			grid = this.uiRoot.transform.FindChild<GridLayoutGroup> ("Grid");
			Point = this.uiRoot.transform.FindChild<Text> ("Point");
			Time = this.uiRoot.transform.FindChild<Text> ("Time");
			table = new UITableManager<UITableItem> ();
			table.InitFromGridLayoutGroup (grid);
			table.Cached = false;

            drag = this.uiRoot.AddComponent<DragRecognizer>();
            drag.OnGesture += (t) =>
            {
                    switch(t.State)
                    {
                        case GestureRecognitionState.Started:
                            break;
                        case GestureRecognitionState.InProgress:
                            {
                                var offset = -t.DeltaMove.y /Screen.height;
                                var res= ThridPersionCameraContollor.Singleton.forward.y +offset;
                                ThridPersionCameraContollor.Singleton.forward.y = Mathf.Clamp(res,-1, -0.05f);
                            }
                            break;
                        case GestureRecognitionState.Ended:
                            break;
                    }
            };
            pinch = this.uiRoot.AddComponent<PinchRecognizer>();
            pinch.OnGesture += (t) =>
            {
                    switch(t.State)
                    {
                        case GestureRecognitionState.InProgress:
                            {
                                var offset = t.Delta /(Screen.height/2);
                                var res= ThridPersionCameraContollor.Singleton.Distance  + offset;
                                ThridPersionCameraContollor.Singleton.Distance = Mathf.Clamp(res,10,25);
                            }
                            break;
                    }
            };
            
		}

        //private float startY = 0;
		private UITableManager<UITableItem> table;
		private Text Point;
		private Text Time;
        private DragRecognizer drag;
        private PinchRecognizer pinch;
		protected GridLayoutGroup grid;
       

		protected override void OnShow ()
		{
			base.OnShow ();




			var datas = ExcelConfig.ExcelToJSONConfigManager.Current.GetConfigs<ExcelConfig.CharacterData> (t => t.ID <= 4);
			table.Count = datas.Length;
			int index = 0;
			foreach (var i in table) {
				var b = i.Root.transform.GetComponent<Button> ();
				var text = i.Root.transform.FindChild<Text> ("Text");
				var Cost = i.Root.transform.FindChild<Text> ("Cost");
				text.text = datas [index].Name;
				Cost.text = datas [index].Cost.ToString ();
				var data = datas [index];
				//only once
				b.onClick.RemoveAllListeners ();
				b.onClick.AddListener (() => {
					OnClick(data);
				});
				index++;
			}
		}

		protected override void OnUpdate ()
		{
			base.OnUpdate ();
			var gate = UAppliaction.Singleton.GetGate() as UGameGate;
			if (gate == null)
				return;
            Point.text = string.Format ("{0:0}", (int)gate.pointLeft);
			var time = System.TimeSpan.FromSeconds (gate.LeftTime);
			Time.text = string.Format ("{0:00}:{1:00}", time.Minutes, time.Seconds);
		}

		private void OnClick (ExcelConfig.CharacterData data)
		{
			//ExcelConfig.CharacterData data =null;
			var gate = UAppliaction.Singleton.GetGate() as UGameGate;
			if (gate == null)
				return;
			gate.CreateCharacter (data);
			
		}
	}
}