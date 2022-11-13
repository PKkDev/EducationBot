using System.Diagnostics;
using System.Text;
using EducationBot.EfData.Entities;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

var pathToDriver = AppContext.BaseDirectory;

var chromeOptions = new ChromeOptions();
//chromeOptions.AddArgument("--headless");
//chromeOptions.PageLoadStrategy = PageLoadStrategy.Default;

Stopwatch stopWatch = new();
stopWatch.Start();

using ChromeDriver driver = new(pathToDriver, chromeOptions, TimeSpan.FromSeconds(60));

TimeZoneInfo samaraTZI = TimeZoneInfo.CreateCustomTimeZone("Samara Time", new(4, 0, 0), "(GMT+04:00) Samara Time", "Samara Time");
List<Lesson> lessonModel = new();

#region from private office

//var url = "https://cabinet.ssau.ru/study/student-timetable";

//var pathToSettings = Path.Combine(AppContext.BaseDirectory, "privatesettings.json");
//var settingsStr = File.ReadAllText(pathToSettings);
//var settingsObj = JObject.Parse(settingsStr);
//var login = settingsObj["login"].ToString();
//var pass = settingsObj["pass"].ToString();

//try
//{
//    driver.Navigate().GoToUrl(url);
//    driver.Manage().Window.Maximize();

//    driver.FindElement(By.Id("__BVID__22")).SendKeys(login);
//    driver.FindElement(By.Id("__BVID__24")).SendKeys(pass);
//    driver.FindElement(By.ClassName("passport-form__button")).Click();

//    (Task.Run(async () => await Task.Delay((int)TimeSpan.FromSeconds(1).TotalMilliseconds))).Wait();

//    driver.Navigate().GoToUrl(url);
//    //driver.Manage().Window.Size = new System.Drawing.Size(1297, 1020);
//    //driver.Manage().Window.Maximize();

//    (Task.Run(async () => await Task.Delay((int)TimeSpan.FromSeconds(1).TotalMilliseconds))).Wait();

//    #region go to first week

//    var goToStartWork = true;
//    while (goToStartWork)
//    {
//        var btnL = driver.FindElements(By.CssSelector(".d-none:nth-child(2)"));
//        if (btnL.Any())
//        {
//            var element = btnL.First();
//            //int elementPosition = element.Location.Y;
//            //driver.ExecuteScript($"window.scroll(0, {elementPosition - elementPosition / 2})");

//            element.Click();
//            (Task.Run(async () => await Task.Delay((int)TimeSpan.FromSeconds(.5).TotalMilliseconds))).Wait();
//        }
//        else
//            goToStartWork = false;
//    }

//    #endregion go to first week

//    var iter = 1;
//    var needDo = true;
//    while (needDo)
//    {
//        List<TypeLesson> typeLessons = new();

//        #region lesson types

//        var lessonTypes = driver.FindElements(By.ClassName("lesson_type"));
//        foreach (var lessonType in lessonTypes)
//        {
//            var classAtr = lessonType.GetAttribute("class");
//            var arr = classAtr.Split(" ");
//            var type = arr.Last();

//            var typeTxt = lessonType.Text;
//            var color = lessonType.GetCssValue("background-color");
//            typeLessons.Add(new TypeLesson(typeTxt, color, type));
//        }

//        #endregion lesson types

//        var weekNavCurrent = driver.FindElement(By.ClassName("current-date"));
//        var weekNow = weekNavCurrent.Text;

//        var dayesRow = driver.FindElement(By.ClassName("days-row"));
//        var dayes = dayesRow.FindElements(By.ClassName("day"));
//        List<WeekDayModel> dayesModel = ParseListDayesFromPrivOff(dayes.ToList());

//        var times = driver.FindElements(By.ClassName("time"));
//        List<LessonTimeModel> timesModel = ParseListTimeCells(times.ToList());

//        var lessonsCells = driver.FindElements(By.ClassName("lessons-cell"));
//        var parsedList = ParseList(lessonsCells.ToList(), 6);

//        var rowCounter = 0;
//        foreach (var row in parsedList)
//        {
//            var timeModel = timesModel[rowCounter];

//            var dayCounter = 0;
//            foreach (var lessonWeb in row)
//            {
//                if (!string.IsNullOrEmpty(lessonWeb.Text))
//                {
//                    var dayModel = dayesModel[dayCounter];

//                    var dateTimeStart = dayModel.Day.Add(timeModel.Start);
//                    var dateTimeStartUtc = TimeZoneInfo.ConvertTimeToUtc(dateTimeStart, samaraTZI);

//                    var dateTimeEnd = dayModel.Day.Add(timeModel.End);
//                    var dateTimeEndUtc = TimeZoneInfo.ConvertTimeToUtc(dateTimeEnd, samaraTZI);

//                    var lessons = lessonWeb.FindElements(By.ClassName("lesson"));
//                    foreach (var lessonElement in lessons)
//                    {
//                        TypeLesson typeLesson = new();
//                        var classAtr = lessonElement.GetAttribute("class");
//                        var arr = classAtr.Split(" ");
//                        var type = arr.Last();
//                        var tempo = typeLessons.FirstOrDefault(x => x.TypeStr == type);
//                        if (tempo != null)
//                            typeLesson = tempo;

//                        //var disciplineL = lessonElement.FindElements(By.ClassName("discipline"));
//                        //var discipline = disciplineL.Any() ? disciplineL.First().Text : null;
//                        var disciplineE = lessonElement.FindElement(By.ClassName("discipline"));
//                        driver.ExecuteScript("arguments[0].style='background-color: green;'", disciplineE);
//                        var discipline = disciplineE.Text;

//                        var roomL = lessonElement.FindElements(By.ClassName("room"));
//                        var room = roomL.Any() ? roomL.First().Text : null;

//                        Teacher teacher = new();
//                        var teacherL = lessonElement.FindElements(By.ClassName("teacher"));
//                        if (teacherL.Any())
//                        {
//                            teacher.Name = teacherL.First().Text;
//                            teacher.Link = teacherL.First().GetAttribute("href");
//                        }

//                        List<string> groups = new();
//                        var scheduleGroups = lessonElement.FindElements(By.ClassName("subgroups"));
//                        var groupTxt = scheduleGroups.Any() ? scheduleGroups.First().Text : null;
//                        if (!string.IsNullOrEmpty(groupTxt))
//                        {
//                            var splitGroups = groupTxt.Split("\r\n");
//                            if (splitGroups.Any())
//                                splitGroups.ToList().ForEach(x => x = x.Trim());
//                            groups.AddRange(splitGroups);
//                        }

//                        string linkToRoom = null;
//                        var linkToRoomL = lessonElement.FindElements(By.XPath("(//a[contains(text(),\'Перейти в конференцию\')])[2]"));
//                        if (linkToRoomL.Any())
//                        {
//                            var text = linkToRoomL.First().Text;
//                            linkToRoom = linkToRoomL.First().GetAttribute("href");
//                        }

//                        LessonModel newLesson = new()
//                        {
//                            Teacher = teacher,
//                            DateTimeStartUtc = dateTimeStartUtc,
//                            DateTimeEndUtc = dateTimeEndUtc,
//                            Day = dayModel,
//                            Time = timeModel,
//                            WeekNunmber = weekNow,
//                            Discipline = discipline,
//                            TypeLesson = typeLesson,
//                            Groups = groups,
//                            Room = room,
//                            LinkToRoom = linkToRoom
//                        };
//                        lessonModel.Add(newLesson);
//                    }
//                }

//                dayCounter++;
//            }

//            rowCounter++;
//        }

//        iter++;
//        needDo = iter < 22;

//        var btnL = driver.FindElements(By.CssSelector(".v-btn__content > .d-none:nth-child(1)"));
//        if (btnL.Any())
//        {
//            var element = btnL.First();
//            element.Click();
//            (Task.Run(async () => await Task.Delay((int)TimeSpan.FromSeconds(.5).TotalMilliseconds))).Wait();
//        }
//        else
//            needDo = false;
//    }
//}
//catch (Exception e)
//{
//    Console.WriteLine(e);
//}
//finally
//{
//    stopWatch.Stop();
//    driver.Close();
//    driver.Dispose();
//}

#endregion from private office

#region from public

var url = "https://ssau.ru/rasp?groupId=755932538&selectedWeek=1&selectedWeekday=1";

try
{
    driver.Navigate().GoToUrl(url);
    driver.Manage().Window.Maximize();

    var iter = 1;
    var needDo = true;
    while (needDo)
    {
        List<TypeLesson> typeLessons = new();

        var timeTableLegendItems = driver.FindElements(By.ClassName("timetable__legend-item"));
        foreach (var timeTableLegendItem in timeTableLegendItems)
        {
            var classAtr = timeTableLegendItem.GetAttribute("class");
            var arr = classAtr.Split("lesson-type-");
            var type = arr.Last().Split("__").First();

            var typeTxt = timeTableLegendItem.Text;
            var color = timeTableLegendItem.GetCssValue("background-color");
            typeLessons.Add(new TypeLesson(typeTxt, color, type));
        }

        var weekNow = driver.FindElement(By.ClassName("week-nav-current")).Text;

        var schedule = driver.FindElement(By.ClassName("schedule"));
        var scheduleItems = schedule.FindElement(By.ClassName("schedule__items"));
        var scheduleItemList = scheduleItems.FindElements(By.XPath("*"));

        var parsedList = ParseList(scheduleItemList.ToList(), 7);

        List<WeekDay> dayesModel = ParseListDayesFromPublic(parsedList.First());

        foreach (var items in parsedList.Skip(1))
        {
            IWebElement timeIntervalElement = items[0];
            LessonTime timeModel = ParseListTimeCellsFromPublic(timeIntervalElement);

            for (var i = 1; i < items.Count; i++)
            {
                var date = dayesModel[i - 1];

                var dateTimeStart = date.Day.Add(timeModel.Start);
                var dateTimeStartUtc = TimeZoneInfo.ConvertTimeToUtc(dateTimeStart, samaraTZI);

                var dateTimeEnd = date.Day.Add(timeModel.End);
                var dateTimeEndUtc = TimeZoneInfo.ConvertTimeToUtc(dateTimeEnd, samaraTZI);

                var lessonElements = items[i].FindElements(By.ClassName("schedule__lesson"));
                foreach (IWebElement lessonElement in lessonElements)
                {
                    TypeLesson typeLesson = new();
                    var classAtr = lessonElement.GetAttribute("class");
                    if (classAtr.Contains("lesson-border-type-"))
                    {
                        var arr = classAtr.Split("lesson-border-type-");
                        var type = arr.Last();
                        var tempo = typeLessons.FirstOrDefault(x => x.TypeStr == type);
                        if (tempo != null)
                            typeLesson = tempo;
                    }

                    var disciplineE = lessonElement.FindElement(By.ClassName("schedule__discipline"));
                    driver.ExecuteScript("arguments[0].style='background-color: green;'", disciplineE);
                    var discipline = disciplineE.Text;

                    var schedulePlace = lessonElement.FindElements(By.ClassName("schedule__place"));
                    var place = schedulePlace.Any() ? schedulePlace.First().Text : null;

                    Teacher teacher = new();
                    var teacherL = lessonElement.FindElements(By.ClassName("schedule__teacher"));
                    if (teacherL.Any())
                    {
                        teacher.Name = teacherL.First().Text;
                        var teacherLinkL = teacherL.First().FindElements(By.TagName("a"));
                        teacher.Link = teacherLinkL.Any() ? teacherLinkL.First().GetAttribute("href") : null;
                    }

                    StringBuilder groups = new();
                    var scheduleGroups = lessonElement.FindElements(By.ClassName("schedule__groups"));
                    var groupTxt = scheduleGroups.Any() ? scheduleGroups.First().Text : null;
                    if (!string.IsNullOrEmpty(groupTxt))
                    {
                        var splitGroups = groupTxt.Split("\r\n");
                        if (splitGroups.Any())
                            splitGroups.ToList().ForEach(x => x = x.Trim());
                        foreach (var gr in splitGroups)
                            groups.AppendLine($"{gr}");
                    }

                    Lesson newLesson = new()
                    {
                        Teacher = teacher,
                        Day = date,
                        WeekNunmber = weekNow,
                        Discipline = discipline,
                        Room = place,
                        Time = timeModel,
                        Groups = groups.ToString(),
                        DateTimeStartUtc = dateTimeStartUtc,
                        DateTimeEndUtc = dateTimeEndUtc,
                        TypeLesson = typeLesson,
                        LinkToRoom = null
                    };
                    lessonModel.Add(newLesson);
                }
            }
        }

        iter++;
        needDo = iter < 22;

        var nextWeekNav = driver.FindElement(By.ClassName("week-nav-next"));
        var link = nextWeekNav.GetAttribute("href");
        driver.Navigate().GoToUrl(link);
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}
finally
{
    stopWatch.Stop();
    driver.Close();
    driver.Dispose();
}

#endregion from public

var totalDiscipline = lessonModel.GroupBy(x => new { x.Discipline, x.TypeLesson.Title }).Select(x => x.Key).Distinct();
foreach (var dis in totalDiscipline)
    Console.WriteLine($"{dis.Discipline} - {dis.Title}");

var lessionStr = JsonConvert.SerializeObject(lessonModel);
Console.ReadKey();

static List<LessonTime> ParseListTimeCells(List<IWebElement> entities)
{
    List<LessonTime> result = new();
    foreach (var entity in entities)
    {
        var startL = entity.FindElements(By.ClassName("start"));
        string? startS = startL.Any() ? startL.First().Text : null;

        var finishL = entity.FindElements(By.ClassName("finish"));
        string? finishS = finishL.Any() ? finishL.First().Text : null;

        if (startS != null && finishS != null)
        {
            result.Add(new LessonTime()
            {
                Start = TimeSpan.Parse(startS),
                End = TimeSpan.Parse(finishS)
            });
        }
    }
    return result;
}
static LessonTime ParseListTimeCellsFromPublic(IWebElement entities)
{
    var timeInterval = entities.Text;
    var split = timeInterval.Split("\r\n");

    var intervalStart = split[0];
    var startS = TimeSpan.Parse(intervalStart);

    var intervalEnd = split[1];
    var finishS = TimeSpan.Parse(intervalEnd);

    return new LessonTime()
    {
        Start = startS,
        End = finishS
    };
}

static List<WeekDay> ParseListDayesFromPrivOff(List<IWebElement> entities)
{
    List<WeekDay> result = new();

    foreach (var entity in entities)
    {
        var dayNameL = entity.FindElements(By.ClassName("day-name"));
        string? dayName = dayNameL.Any() ? dayNameL.First().Text : null;

        var dateL = entity.FindElements(By.ClassName("date"));
        string? date = dateL.Any() ? dateL.First().Text : null;

        if (dayName != null && date != null)
        {
            var dayOfWeek = dayName;
            var dateParse = Convert.ToDateTime(date);

            result.Add(new WeekDay(dateParse.Date, dayOfWeek));
        }
    }

    return result;
}

static List<WeekDay> ParseListDayesFromPublic(List<IWebElement> entities)
{
    List<WeekDay> result = new();

    foreach (var header in entities)
    {
        var scheduleHeadWeekday = header.FindElements(By.ClassName("schedule__head-weekday"));
        string? weekdayTxt = scheduleHeadWeekday.Any() ? scheduleHeadWeekday.First().Text : null;

        var scheduleHeadDate = header.FindElements(By.ClassName("schedule__head-date"));
        string? dateTxt = scheduleHeadDate.Any() ? scheduleHeadDate.First().Text : null;

        if (weekdayTxt != null && dateTxt != null)
        {
            var dayOfWeek = weekdayTxt;
            var date = Convert.ToDateTime(dateTxt);

            result.Add(new WeekDay(date.Date, dayOfWeek));
        }
    }

    return result;
}


static List<List<IWebElement>> ParseList(List<IWebElement> entities, int baseSkip)
{
    List<List<IWebElement>> list = new();

    var needDo = entities.Any();
    var skip = 0;

    while (needDo)
    {
        var map = entities.Skip(skip).Take(baseSkip).ToList();
        skip += baseSkip;

        needDo = map.Any();
        if (needDo)
            list.Add(map);
    }

    return list;
}


/*
 
прикладные информационные системы	Практика
Современные методы разработки и проектирования программных комплексов	Лабораторная
Нейронные сети	Лабораторная
прикладные информационные системы	Лекция
интеллектуальный анализ и большие данные	Практика
Современные методы разработки и проектирования программных комплексов	Лекция
Современные методы разработки и проектирования программных комплексов	Практика
Иностранный язык в профессиональной сфере	Лабораторная
Data Mining and Big Data (Интеллектуальный анализ данных и большие данные)	Лекция
Информационная безопасность корпоративных систем	Практика
интеллектуальный анализ и большие данные	Лабораторная
прикладные информационные системы	Лабораторная
Информационная безопасность корпоративных систем	Лекция
Интеллектуальный анализ данных	Лабораторная
Нейронные сети	Лекция
Командообразование в проектной и исследовательской деятельности	Лекция
Нейронные сети	Практика
Командообразование в проектной и исследовательской деятельности	Практика
Информационная безопасность корпоративных систем	Лабораторная
 
 */