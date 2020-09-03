# 
# 这里把makefile当作脚本用了
#  如果你没有make程序, 就把对应的指令复制到终端里执行
# 

# 删库(跑路)
drop:
	dotnet ef migrations remove
	dotnet ef database drop

# 建库
create:
	dotnet ef migrations add InitialCreate
	dotnet ef database update

# 删完重建
rebuild: drop create
