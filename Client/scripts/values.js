class Value {
  constructor(name, type, defaultValue) {
    this.name = name;
    this.type = type;
    this.defaultValue = defaultValue;
  }
}

const valueInfo = [
  new Value("Год", "number", 2017),
  new Value("Месяц", "number", 5),
];

const values = {};
for (const info of valueInfo) {
  values[info.name] = info;
}

export default values;
