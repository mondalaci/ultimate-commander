#!/usr/bin/env python

import pygtk
pygtk.require('2.0')

from MainWindow import *
import Colors

Colors.init()

main_window = MainWindow()
main_window.show_all()

gtk.main()
