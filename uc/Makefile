objects = color.o fileentry.o filelist.o frame.o globals.o input.o widget.o mem.o panel.o uc.o

uc: $(objects)
	gcc -lncurses -o uc $(objects)

color.o: color.h
fileentry.o: fileentry.h
filelist.o: filelist.h
frame.o: frame.h
globals.o: globals.h
input.o: input.h
widget.o: widget.h
mem.o: mem.h
panel.o: panel.h
uc.o: uc.h frame.h

.PHONY: clean
clean:
	rm -f *.o uc
