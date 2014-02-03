Starbound-Planet-Scanner
========================

![Screenshot of my scanner](http://showmethebitcoin.com/wp-content/uploads/2014/02/SBDBScreenShot.png "Screenshot")


Finally, an automatic way to log planets with no typing! Using a custom OCR (optical character recognition) engine, I've created a program that can interpret screenshots of the Starbound Planet Navigator. Simply open Starbound, open the navigator, mouse over the planet you want to log and hit print-screen. If you hear a ding, it worked!

Once the planets are logged in a sortable spreadsheet notes can be added or details can be changed.


**Features:**
- Automatically reads X, Y, Biome, Threat, Name and System from the Starbound application
- Sort-able and editable spreadsheet view
- Import/Export features featuring easy to use XML files
- Export format can be used by other planet log sites/apps

**Limitations:**
- Currently windows only, also requires .NET 4.5
- If you're using a mac in bootcamp, you need to hit shift-fn-f11 instead of print screen.
- This program scans the active monitor when print screen is pressed, it will not work if Starbound is minimized or on a secondary monitor.
- The mouse cursor must be over the targeted planet (otherwise it just logs the system.

**Installation:**
- Extract and enjoy! You might need to install .NET Framework 4.5, but it will prompt you.
- If upgrading, copy your data and planetpics folders into the new directory before running.

Want more features? Maybe buy me a beer and we can talk about it:

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=HRTFZC93YFJRY)

**Have bitcoins?** THAT'S SUPER. 

If you find my tool useful and want me to know it, please consider tipping some bitcoin to **16K9awt7tfdgiWB79qzbSH4wpfoU3VS8zP**

I just did this for fun and I'm only one person, thanks for checking it out!

-- Nick


Here's a full view of starbound and the scanner actually working:
![Shot of both screens at once](http://showmethebitcoin.com/wp-content/uploads/2014/02/SBDBActionShot.png "Action shot")

========================
**Want to add/change the fields available?**

Close the scanner application and edit the XML file located at data/sbdb.xml; Add any new data nodes you want, just make sure the planet name is always the second one.

For instance, if you wanted to add "In Progress" and "Done" to the list of fields, just add `<inProgress /><done />` before the first `</sbplanet>` closing tag.

I'll probably add a GUI for this eventually...

========================
**Upgrading from another build?**
Just copy your "data" and "planetpics" folders from the old build folder to the new build folder. All user data is in those two folders.
