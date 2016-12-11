// Hello World!  Your first WinIOT project with the GrovePi!
/*

This example simply turns and LED on and off at 1 second intervals.  

The GrovePi connects the Raspberry Pi and Grove sensors.  
You can learn more about GrovePi here:  http://www.dexterindustries.com/GrovePi

This example combines the GrovePi + LED:
    http://www.dexterindustries.com/shop/grovepi-board/
    http://www.dexterindustries.com/shop/grove-green-led/

Hardware Setup:
    Connect the LED to digital port 2

*/

/*
The MIT License(MIT)
GrovePi for the Raspberry Pi: an open source platform for connecting Grove Sensors to the Raspberry Pi.
Copyright (C) 2016  Dexter Industries
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using GrovePi;
using GrovePi.Sensors;

// GrovePi Libraries Needed

namespace HelloWorldBlinky
{
    public sealed class StartupTask : IBackgroundTask
    {
        private IBuzzer buz;
        private ILed led;
        private IRotaryAngleSensor angleSensor;
        private IButtonSensor button;

        private string qgstr = "";

        private readonly QueueGate queueGate = new QueueGate(10, "", "", "");

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            buz = DeviceFactory.Build.Buzzer(Pin.DigitalPin2);
            angleSensor = DeviceFactory.Build.RotaryAngleSensor(Pin.AnalogPin2);
            led = DeviceFactory.Build.Led(Pin.DigitalPin3);
            led.ChangeState(SensorStatus.Off);
            button = DeviceFactory.Build.ButtonSensor(Pin.DigitalPin4);

            var dis = DeviceFactory.Build.RgbLcdDisplay();

            bool isModeListening = true;

            while (true)
            {
                isModeListening = angleSensor.SensorValue() < 512 ? true : false;
                if (isModeListening)
                {
                    dis.SetText("Listening...").SetBacklightRgb(0, 0, 255);
                    Task.Delay(500).Wait();
                    qg().Wait();
                    if (qgstr != null)
                    {
                        dis.SetText(qgstr).SetBacklightRgb(0, 255, 0);

                        foreach (var c in qgstr)
                            Beep(c);
                    }
                }
                else
                {
                    dis.SetText("Sending...").SetBacklightRgb(255, 0, 255);
                    
                    bool flag = false;
                    long start = DateTime.Now.Ticks;
                    while (true)
                    {
                        

                        if (angleSensor.SensorValue() < 512) break;

                        if (!flag && button.CurrentState == SensorStatus.On)
                        {
                            flag = true;
                            buz.ChangeState(SensorStatus.On);
                            led.ChangeState(SensorStatus.On);
                            //Debug.WriteLine(flag);
                            times.Add(DateTime.Now.Ticks);
                            start = DateTime.Now.Ticks;
                        }
                        else if(flag && button.CurrentState == SensorStatus.Off)
                        {
                            flag = false;
                            buz.ChangeState(SensorStatus.Off);
                            led.ChangeState(SensorStatus.Off);
                            //Debug.WriteLine(flag);
                            times.Add(DateTime.Now.Ticks);
                            start = DateTime.Now.Ticks;
                        }

                        /////////////////////////////////////////////////////
                        //Debug.WriteLine(DateTime.Now.Ticks - start);
                        if (DateTime.Now.Ticks - start > TimeSpan.TicksPerMillisecond*3000)
                        {
                            for (int i = 0; i < times.Count - 1; i++)
                            {
                                times2.Add((times[i+1] - times[i])/TimeSpan.TicksPerMillisecond);
                            }

                            for (int i = 0; i < times2.Count; i+=2)
                            {
                                generatedMorse += times2[i] > 300 ? '_' : '.';
                            }

                            //Debug.WriteLine(generatedMorse);
                            if (generatedMorse != "")
                            {
                                if (!bylStart)
                                {
                                    SendMorse("*").Wait();
                                    bylStart = true;
                                }
                                SendMorse(generatedMorse).Wait();
                            }
                            else
                            {
                                if(bylStart)
                                    SendMorse("#").Wait();
                                bylStart = false;
                            }

                            times.Clear();
                            times2.Clear();
                            generatedMorse = "";

                            break;
                        }
                    }
                }
            }
        }

        private bool bylStart;

        private List<long> times = new List<long>();
        private List<long> times2 = new List<long>();
        private string generatedMorse = "";

        private async Task SendMorse(string str)
        {
            Debug.WriteLine("sending: " + str);
            await queueGate.SendMessageOperation(str);
        }

        private async Task qg()
        {
            qgstr = await queueGate.GetLastMessageOperation();
        }

        private int len = 75;

        private void Beep(char c)
        {
            if (c == '.')
            {
                buz.ChangeState(SensorStatus.On);
                led.ChangeState(SensorStatus.On);
                Task.Delay(len).Wait();
                buz.ChangeState(SensorStatus.Off);
                led.ChangeState(SensorStatus.Off);
                Task.Delay(len).Wait();
            }
            else if (c == '_')
            {
                buz.ChangeState(SensorStatus.On);
                led.ChangeState(SensorStatus.On);
                Task.Delay(len*3).Wait();
                buz.ChangeState(SensorStatus.Off);
                led.ChangeState(SensorStatus.Off);
                Task.Delay(len).Wait();
            }
            else if (c == ' ')
            {
                Task.Delay(len).Wait();
            }
            else if (c == '|')
            {
                Task.Delay(len*7).Wait();
            }
        }
    }
}