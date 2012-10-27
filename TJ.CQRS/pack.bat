FOR /F "tokens=*" %%i in ('dir /X /B /O:-D') DO (
	ECHO '%%i'...
)


::nuget pack *.csproj -prop Configuration=Release -symbols