objects = color.o fileentry.o filelist.o frame.o input.o link.o mem.o panel.o uc.o

uc: $(objects)
	gcc -lncurses -o uc $(objects)

color.o: color.h
fileentry.o: fileentry.h
filelist.o: filelist.h
frame.o: frame.h
input.o: input.h
link.o: link.h
mem.o: mem.h
panel.o: panel.h
uc.o: uc.h frame.h

.PHONY: clean
clean:
	rm -f *.o uc
