import gtk

from gtkpanel import *
import gtkbasic

gtkbasic.init_colors()
window = gtk.Window()
window.set_title('The Ultimate Commander')
window.connect('destroy', lambda widget: gtk.main_quit())
panel=Panel()
window.add(panel.widget)
window.set_default_size(400, 700)
window.show_all()
gtk.main()
