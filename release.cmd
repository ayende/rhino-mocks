del ..\build\net-3.5\debug\Rhino.Mocks.???
nant zip -D:version=v3.5 
del ..\build\net-3.5\debug\Rhino.Mocks.???
nant zip -D:version=v3.5-for-2.0 -D:build.defines=FOR_NET_2_0
del ..\build\net-3.5\debug\Rhino.Mocks.???
nant zip -D:skip.merge.asm=true -D:version=v3.5-unmerged
del ..\build\net-3.5\debug\Rhino.Mocks.???
nant zip -D:skip.merge.asm=true -D:version=v3.5-for-2.0-unmerged -D:build.defines=FOR_NET_2_0

