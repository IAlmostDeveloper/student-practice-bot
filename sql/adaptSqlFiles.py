from os import listdir

names = [f for f in listdir() if f.endswith(".sql")]
for file in names:
	text = open(file).read().replace("utf8mb4_0900_ai_ci", "utf8mb4_general_ci")
	open(file, "w").write(text)
print("Files adapted successfully")