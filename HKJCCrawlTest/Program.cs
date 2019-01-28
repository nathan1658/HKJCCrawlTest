using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKJCCrawlTest
{
    class Program
    {
        static void Main(string[] args)
        {










            List<RaceDetail> raceDetailList = new List<RaceDetail>();

            IWebDriver driver = new ChromeDriver();
            driver.Url = @"https://racing.hkjc.com/racing/SystemDataPage/racing/ResultsAll-iframe-SystemDataPage.aspx?match_id=20170906&lang=Chinese";
            var results = driver.FindElements(By.XPath("//div[contains(@class, 'raceNum') and contains(@class, 'rowDiv15')]"));
            foreach(var tmp in results.Where(x => x.Displayed == true))
            {
                var result = createRaceDetail(tmp.Text);
                raceDetailList.Add(result);
                //var raceNum = tmp.FindElement(By.XPath("//div/div[2]/table/tbody/tr[1]/td/div[1]")).Text;
                //raceNum = raceNum.Replace("第", "").Replace("場", "").Replace(" ","");
            }
            File.WriteAllText(@".\test.txt", JsonConvert.SerializeObject(raceDetailList, Formatting.Indented));
            
            

        }


        static RaceDetail createRaceDetail(string example)
        {


            var array = example.Split('\n');
            var raceNum = array[0].Replace("第", "").Replace("場", "").Replace(" ", "").Replace("\r", "");
            RaceDetail raceDetail = new RaceDetail();
            var raceDetailArr = array[1].Split(new string[] { " - " }, StringSplitOptions.None);
            raceDetail.Class = raceDetailArr[0];
            raceDetail.Distance = raceDetailArr[1];
            raceDetail.Track = raceDetailArr[3];
            raceDetail.Handicap = raceDetailArr[5].Replace(" ", "").Replace("\r", "");

            Func<string, HorseRecord> getHorseRecord = new Func<string, HorseRecord>(x =>
            {
                HorseRecord tmpRecord = new HorseRecord();
                string[] tmpArr = x.Split(' ');
                tmpRecord.Place = int.Parse(tmpArr[0]);
                tmpRecord.HorseNo = int.Parse(tmpArr[1]);
                tmpRecord.HorseName = tmpArr[2];
                tmpRecord.Jockey = tmpArr[3];
                tmpRecord.Trainer = tmpArr[4];
                tmpRecord.ActualWeight = tmpArr[5];
                tmpRecord.Draw = tmpArr[6].Replace(" ", "").Replace("\r", "");
                return tmpRecord;
            });
            raceDetail.HorseRecordList = new List<HorseRecord>();
            raceDetail.HorseRecordList.Add(getHorseRecord(array[5]));
            raceDetail.HorseRecordList.Add(getHorseRecord(array[6]));
            raceDetail.HorseRecordList.Add(getHorseRecord(array[7]));
            raceDetail.HorseRecordList.Add(getHorseRecord(array[8]));
            return raceDetail;
        }
    }

    class RaceDetail
    {
        public string Class { get; set; }
        public string Distance { get; set; }
        /// <summary>
        /// TURF(草地)
        /// </summary>
        public string Track { get; set; }
        /// <summary>
        /// XXX賽
        /// </summary>
        public string Handicap { get; set; }

        public IList<HorseRecord> HorseRecordList { get; set; }
        public IList<Dividend> DividendList { get; set; }
    }

    class HorseRecord
    {
        /// <summary>
        /// 名次
        /// </summary>
        public int Place { get; set; }
        public int HorseNo { get; set; }
        public string HorseName { get; set; }
        public string Jockey { get; set; }
        public string Trainer { get; set; }
        public string ActualWeight { get; set; }
        /// <summary>
        /// 檔位
        /// </summary>
        public string Draw { get; set; }
    }

    class Dividend
    {
        public string PoolName { get; set; }
        public int[] WinningCombination{ get; set; }
        public string WinningCombinationString
        {
            get
            {
                if(WinningCombination!=null && WinningCombination.Length>0)
                {
                    return String.Join(",",WinningCombination);
                }
                return "";
            }
        }

        public double DividendPrize { get; set; }
    }


}
