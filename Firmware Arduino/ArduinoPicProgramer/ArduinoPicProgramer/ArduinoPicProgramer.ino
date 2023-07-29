#include <stdlib.h>
//region declaracion de variables
int VppIn = A0;    // Pin para el sensado del voltaje de programacion
int PinRef = A1;    // Pin para obtener el voltaje de referencia de 5 v
int pwmvalue = 0;
int PINPWM = 5;
int PinVpp = 4;
int PinData = 2;
int PinClock = 7;
float relacionConversion = 1;
float Vpp=13; //13 volts por default
int TimeDelayPulce=100;
//endregion
//region Tabla de comandos basicos
#define CORE_INSTRUCCION B0000
#define SHITF_OUT_REGISTER B0010
#define TABLE_READ B1000
#define TABLE_READ_POST_INCREMENT B1001
#define TABLE_READ_POS_DECREMENT B1010
#define TABLE_READ_PRE_INCREMENT B1011
#define TABLE_WRITE B1100
#define TABLE_WRITE_POS_INCREMENT B1101
#define TABLE_WRITE_START_PROGRAMIN_POST_INCREMENT B1101
#define TABLE_WRITE_START_PROGRAMIN B1111
//endregion
//region setup
void setup()
{
  // put your setup code here, to run once:
  pinMode(PinData, OUTPUT); //datos como salida
  pinMode(PinClock, OUTPUT); //reloj como salida
  pinMode(PinVpp, OUTPUT); //pinde voltaje de programacion como salida
  pinMode(VppIn, INPUT); //
  pinMode(PinRef, INPUT);
  pinMode(PINPWM, OUTPUT);
  TCCR0B = TCCR0B & B11111000 | B00000001; // coloca la frecuencia del pwm en 600
  analogWrite(PINPWM, pwmvalue); //crea una onda con cilco util de 50%
  pinMode(LED_BUILTIN, OUTPUT);
  Serial.begin(115200);//19200);//19200);
  Serial.println("Hola oscar");
  //analogReference(INTERNAL);
 Calibra();
  Regula(Vpp);
}
//endregion
//region funciones de tiempo
void delay_ms(unsigned long ret)//ok
{
  delay(ret * 64);
}
//tiempo de espera entre cambio de estado
void DelayPulso(int pulsos = 1) //ok
{
  for (int i = 0; i < pulsos; i++);
  {
    delayMicroseconds(TimeDelayPulce);
  }
}
//endregion
//region control de la generacion del voltaje de programacion
//obtiene la relacion de cconversion para hacr la conversion a voltaje
void Calibra() //ok
{
  float ref5v = 0;
  //leo la entrada de refreencia
  for (int i = 0; i < 100; i++)
  {
    ref5v += analogRead(PinRef);
    delay_ms(1);
  }
  ref5v = ref5v / 100;
  //ahora saco la relacion de conversion
  relacionConversion = 5 / ref5v;
}
//lee la entrada analogica y regresa el voltaje
float GetVoltaje() //ok
{
  float sensorValue = 0;
  sensorValue = 0;
  for (int i = 0; i < 100; i++)
  {
    sensorValue += analogRead(VppIn);
    //delay_ms(1);
  }
  sensorValue = sensorValue / 100;
  float v = sensorValue * relacionConversion;
  return v;
}
//regula el voltaje especificado y regresa hasta que alcansa el valor mas cercan a ese voltaje 
void Regula(float voltajex) //ok
{
  VppOn();
  int ok=false;
  do
  {
    float v = GetVoltaje();
//    Serial.println("v= "+String(v)+" voltajex= "+String(voltajex)+" pwmvalue="+String(pwmvalue));
    float dif = 0;
    if((voltajex)+.1>=v &&(voltajex)-.1<=v)//aceptable un rango de mas menos medio volt
    {
      VppOff();
      return;
    }
    if (v < voltajex)
    {
      dif = (v - voltajex);
      if (dif < 1)
        dif = 1;
      pwmvalue += dif;
    }
    else if (v > voltajex)
    {
      dif = (voltajex - v);
      if (dif < 1)
        dif = 1;
      pwmvalue -= dif;
    }
    analogWrite(PINPWM, pwmvalue); //crea una onda con cilco util ajustado
    delay_ms(5);
  }
  while(ok==false);
  VppOff();
}
//endregion
//region uso de pines basicos
//enciende el voltaje de programacion
void VppOn() //ok
{
  digitalWrite(PinVpp, LOW);
}
//apaga el voltaje de programacion
void VppOff()//ok
{
  digitalWrite(PinVpp, HIGH);
}
//pone a 1 el pin de datos
void DataOn()//ok
{
  digitalWrite(PinData, HIGH);
}
//pone a 0 el in de datos
void DataOff()//ok
{
  digitalWrite(PinData, LOW);
}
//pone a 1 el pin de relog
void ClockOn()//ok
{
  digitalWrite(PinClock, HIGH);
}
//poce a 0 el bit de relog
void ClockOff()//ok
{
  digitalWrite(PinClock, LOW);
}
//coloca el pin de datos en modo entrada
void DataModeInput() //ok
{
  pinMode(PinData, INPUT); //
}
// coloca el pin de datos en modo salida
void DataModeOutput() //ok
{
  pinMode(PinData, OUTPUT); //
}
//endregion
//region comandos basicos
// entra en modo de programacion
void InProgramMode() //ok
{
  ClockOff();
  DataOff();
  VppOn();
}
//sale del modo de programacion
void OutProgramMode()//ok
{
  ClockOff();
  DataOff();
  VppOff();
}
//pone el bit y genera un pulso de relog
void SendBit(int b)//ok
{
  ClockOn();
  if (b == 1)
  {
    DataOn();
  }
  else
  {
    DataOff();
  }
  DelayPulso();
  ClockOff();
  DelayPulso();
}
// manda los datos de forma serial desde el bits menos significativo al mas significativo indicando el numero de bits a enviar
void SendBits(char datos, int nbits = 8)//ok
{
  char tmp = datos;
 	//envia desde el bit menos significativo al mas significacito
 	for (int i = 0; i < nbits; i++)
 	{
   	if (tmp & 0x01 == 1) //Veo si el bit menos significativo es 1 o 0
   	{
     	SendBit(1); //es 1
    }	
   	else
    {
   	  SendBit(0); //es 0
    }
   	//hago un corrimiento a la derecha
    tmp = tmp >> 1;
 	}
}

//envia el comando y dos bytes de datos
void SendCommand(char comando, char hiData, char lowData) //ok
{
  char tmp;
  //el comando corresponde de 4 bits y se manda del bit menos significativo al mas significativo
  SendBits(comando, 4);
  //ahora el byte menos significativo
  SendBits(lowData, 8);
  //ahora mando el byte mas significativo
  SendBits(hiData, 8);
  delay_ms(1);
}
//lee un bit de datos durante un pulso
char ReadBit() //ok
{
  char c = 0x00;
  ClockOn();
  DelayPulso();
  c = digitalRead(PinData);
  ClockOff();
  DelayPulso();
  return c;
}
//lee nbits del pin de datos
int ReadBits(int nbits = 8) //ok
{
  int c = 0;
  int data = 0;
  //pongo el bit de datos de lectura
  DataModeInput();
  //leo los bits leyendo primero el bit menos significativo y al final el bit mas significativo
  for (int i = 0; i < nbits; i++)
  {
    //leo el bit
    c = ReadBit();
    //recorro los bits a la izquierda para asignar el ponderamiento del bit
    c = c << i;
    data = data | c;
  }
  //establesco el bot de datos de salida
  DataModeOutput();
  return data;
}
//endregion
//region comandos extendidos
//region Escrituras de memoria
//borra todo el chip
void ErraceChip() //ok
{
  InProgramMode();
  SendCommand(CORE_INSTRUCCION, 0x0E, 0x3C);
  SendCommand(CORE_INSTRUCCION, 0X6E, 0XF8);
  SendCommand(CORE_INSTRUCCION, 0X0E, 0X00);
  SendCommand(CORE_INSTRUCCION, 0X6E, 0XF7);
  SendCommand(CORE_INSTRUCCION, 0X0E, 0X05);
  SendCommand(CORE_INSTRUCCION, 0X6E, 0XF6);
  SendCommand(TABLE_WRITE, 0X3F, 0X3F);
  SendCommand(CORE_INSTRUCCION, 0X0E, 0X3C);
  SendCommand(CORE_INSTRUCCION, 0X0E, 0XF8);
  SendCommand(CORE_INSTRUCCION, 0X0E, 0X00);
  SendCommand(CORE_INSTRUCCION, 0X6E, 0XF7);
  SendCommand(CORE_INSTRUCCION, 0X0E, 0X04);
  SendCommand(CORE_INSTRUCCION, 0X6E, 0XF6);
  SendCommand(TABLE_WRITE, 0X8F, 0X8F);
  SendCommand(CORE_INSTRUCCION, 0X00, 0X00);
  SendCommand(CORE_INSTRUCCION, 0X00, 0X00);
  OutProgramMode();
}
//la direccion de memoria esta dada por HDir MDir LDir y los datos estan dados por HData LData
void WriteConfigMemory(char HDir, char MDir, char LDir, char Hdata, char LData) //ok
{
  InProgramMode();
  //paso 1: Acceso directo a la memoria de codigo y habilitar escritura
  SendCommand(CORE_INSTRUCCION, 0x8E, 0xA6);
  SendCommand(CORE_INSTRUCCION, 0x8C, 0xA6);
  //pASO 2: CARGAR LA DIRECCION EN EL BUFFER DE ESCRITURA
  SendCommand(CORE_INSTRUCCION, 0x0E, HDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF8);
  SendCommand(CORE_INSTRUCCION, 0x0E, MDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF7);
  SendCommand(CORE_INSTRUCCION, 0x0E, LDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF6);
  //PASO 3: ESCIBIR OS DATOS
  SendCommand(TABLE_WRITE_START_PROGRAMIN, Hdata, LData);
  //hay que mandar 20 bits en 0
  //primero mando 3
  SendBits(0x00,3);
  //En el 4 bit PONGO EL RELOJ EN ALTO para que inicie la programacion
  ClockOn();
  delay_ms(5);
  ClockOff();
  delay_ms(5);
  //ahora mando los 16 bits que faltan
  SendBits(0x00,8);
  SendBits(0x00,8);

  OutProgramMode();
//  InProgramMode();
//nado el segundo byte 
//  SendCommand(CORE_INSTRUCCION, 0x0E, 0x01);
  //SendCommand(CORE_INSTRUCCION, 0x6E, 0xF6);
//  SendCommand(TABLE_WRITE_START_PROGRAMIN, Hdata, LData);
  //primero mando 3
//  SendBits(0x00,3);
  //En el 4 bit PONGO EL RELOJ EN ALTO para que inicie la programacion
//  ClockOn();
//  delay_ms(5);
//  ClockOff();
  //delay_ms(5);
  //ahora mando los 16 bits que faltan
//  SendBits(0x00,8);
  //SendBits(0x00,8);
//  OutProgramMode();
}
//Escribe datos en la memoria de programa
//la direccion de memoria esta dada por HDir MDir LDir y los datos estan dados por HData LData
void WriteProgramMemory(char HDir, char MDir, char LDir, char Hdata, char LData) //ok
{
  InProgramMode();
  //paso 1: Acceso directo a la memoria de codigo y habilitar escritura
  SendCommand(CORE_INSTRUCCION, 0x8E, 0xA6);
  SendCommand(CORE_INSTRUCCION, 0x9C, 0xA6);
  //pASO 2: CARGAR LA DIRECCION EN EL BUFFER DE ESCRITURA
  SendCommand(CORE_INSTRUCCION, 0x0E, HDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF8);
  SendCommand(CORE_INSTRUCCION, 0x0E, MDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF7);
  SendCommand(CORE_INSTRUCCION, 0x0E, LDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF6);
  //PASO 3: ESCIBIR OS DATOS
  SendCommand(TABLE_WRITE_START_PROGRAMIN, Hdata, LData);
  //SendCommand(CORE_INSTRUCCION, 0x00, 0x00);
  //hay que mandar 20 bits en 0
  //primero mando 3
  SendBits(0x00,3);
  //En el 4 bit PONGO EL RELOJ EN ALTO para que inicie la programacion
  ClockOn();
  delay_ms(5);
  ClockOff();
  delay_ms(5);
  //ahora mando los 16 bits que faltan
  SendBits(0x00,8);
  SendBits(0x00,8);
  OutProgramMode();
}
//escibe en la memoria EEPROM de datos
void WriteDataMemory(char HDir, char LDir, char Data) //ok
{
  int escribiendo = 0;
  InProgramMode();
  //paso 1: Acceso directo a datos EEPROM
  SendCommand(CORE_INSTRUCCION, 0x9E, 0xA6);
  SendCommand(CORE_INSTRUCCION, 0x9C, 0xA6);
  //paso 2: establecer la direccion de memoria EEPROM
  SendCommand(CORE_INSTRUCCION, 0x0E, LDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xA9);
  SendCommand(CORE_INSTRUCCION, 0x0E, HDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xAA);
  // paso 3: cargar el byte que se va a escribir
  SendCommand(CORE_INSTRUCCION, 0x0E, Data);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xA8);
  //paso 4: habilitar escritura de la memoria
  SendCommand(CORE_INSTRUCCION, 0x84, 0xA6);
  //PASO 5: Iniciar la escritura
  SendCommand(CORE_INSTRUCCION, 0x82, 0xA6);
  escribiendo = 1;
  //paso 6: verificar el bit WR de EECON1<1> si esta a 1 significa que aun no termina de grabarse el datos
  do
  {
    SendCommand(CORE_INSTRUCCION, 0x50, 0xA6);
    SendCommand(CORE_INSTRUCCION, 0x6E, 0xF5);
    SendCommand(CORE_INSTRUCCION, 0x00, 0x00);
    SendBits(SHITF_OUT_REGISTER);
    DataOff();
    for (int i = 0; i < 8; i++)
    {
      ClockOn();
      DelayPulso();
      ClockOff();
      DelayPulso();
    }
    char c = ReadBits(); //leo los bits
    c = c & 0X01; //Aplico una mascara para que solo tome en cuenta el bit 1 de EECON1
    if (c == 0)
    {
      //ya es cero por lo que ya termino de escribir
      escribiendo = 0;
    }
  }
  while (escribiendo == 1);
  // paso 7: mantener el pin de relog en bajo por un tiempo
  ClockOff();
  delay_ms(1);
  //paso 8: deshabilitar la escritura
  SendCommand(CORE_INSTRUCCION, 0x94, 0xA6);
  OutProgramMode();
}
//Graba los bits de configuracion
void WriteConfigurationBits(char HConfig, char LConfig) //ok
{
  InProgramMode();
  //paso 1: habilita la escritura y acceso a la memoria de configuracion
  SendCommand(CORE_INSTRUCCION, 0x8E, 0xA6);
  SendCommand(CORE_INSTRUCCION, 0x8C, 0xA6);
  //paso 2: Configura el apuntador de tabla para que escriba el byte de configuracion. Escriba direcciones pares / impares
  SendCommand(CORE_INSTRUCCION, 0x0E, 0x30);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF8);
  SendCommand(CORE_INSTRUCCION, 0x0E, 0x00);
  SendCommand(CORE_INSTRUCCION, 0x0E, 0xF7);
  SendCommand(CORE_INSTRUCCION, 0x0E, 0x00);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF6);
  SendCommand(TABLE_WRITE_START_PROGRAMIN, 0x00, LConfig);
  SendCommand(CORE_INSTRUCCION, 0x00, 0x00);
  SendCommand(CORE_INSTRUCCION, 0x0E, 0x01);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF6);
  SendCommand(TABLE_WRITE_START_PROGRAMIN, HConfig, 0x00);
  SendCommand(CORE_INSTRUCCION, 0x00, 0x00);
  OutProgramMode();
}
//endregion
//region Lecturas de memoria
//regresa el byte que se encuentra en la memoria definida por H,M,l Dir
char ReadProgramMemory(char HDir, char MDir, char LDir) //ok
{
  InProgramMode();
  //paso 1: establecer el apuntador de tabla con la direccion a leer
  SendCommand(CORE_INSTRUCCION, 0x0E, HDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF8);
  SendCommand(CORE_INSTRUCCION, 0x0E, MDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF7);
  SendCommand(CORE_INSTRUCCION, 0x0E, LDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF6);
  //paso 2: lee la memoria
  SendBits(TABLE_READ_POST_INCREMENT, 4);
  //se tiene que mandar 8 pulsos de relog con el pin de datos en bajo
  DataOff();
  for (int i = 0; i < 8; i++)
  {
    ClockOn();
    DelayPulso();
    ClockOff();
    DelayPulso();
  }
  char c = ReadBits(); //Ahora leo los datos
  OutProgramMode();
  return c;
}
//regresa el dato almacenado en la EEPROM
char ReadDataMemory(char HDir, char LDir) //ok
{
  InProgramMode();
  //paso 1: Acceso directo a datos EEPROM
  SendCommand(CORE_INSTRUCCION, 0x9E, 0xA6);
  SendCommand(CORE_INSTRUCCION, 0x9C, 0xA6);
  //Paso 2: Configurar el apuntador con la direccion a leer
  SendCommand(CORE_INSTRUCCION, 0x0E, LDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xA9);
  SendCommand(CORE_INSTRUCCION, 0x0E, HDir);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xAA);
  //Paso 3: Iniciar una lectura de memoria
  SendCommand(CORE_INSTRUCCION, 0x80, 0xA6);
  // Paso 4: cargar datos en el regirtso de retencion de datos en serie
  SendCommand(CORE_INSTRUCCION, 0x50, 0xA8);
  SendCommand(CORE_INSTRUCCION, 0x6E, 0xF5);
  SendCommand(CORE_INSTRUCCION, 0x00, 0x00);
  SendBits(SHITF_OUT_REGISTER, 4);
  //se tiene que mandar 8 pulsos de relog con el pin de datos en bajo
  DataOff();
  for (int i = 0; i < 8; i++)
  {
    ClockOn();
    DelayPulso();
    ClockOff();
    DelayPulso();
  }
  //Ahora leo los datos
  char c = ReadBits();
  OutProgramMode();
  return c;
}
//endregion
//endregion
//region API para la PC
//los servicios a exponer son los siguientes
//1. Borrar chip 
//2. Escribir memoria de programa
//3. Escribir memoria EEPROM
//4. Leer memoria de programa
//5. Leer EEPROM
//6. escribir configuracion
// la cadena que se va a recibir es la siguiente: :comando,parametro1,parametro2,parametro3,parametro4,parametro5;
//region variables para los comandos
String Comando;
char Parametro1;
char Parametro2;
char Parametro3;
char Parametro4;
char Parametro5;
//endregion
//lee datos desde el puerto serial y regresa la cadena recibida
// la cadena empieza con el caracter ':' y termina con ';'
String RecibeComando()//ok
{
	String s="";
	//me espero hasta recibir datos del puerto serial
	do
	{
		//me espero hasta que se tengan datos del puerto serial
		while (Serial.available()==0);
		//leo un caracter
		char c=Serial.read(); 
		if(c==';')
		{
			//se termino de recibir el comando
			s=s+";";
			return s;
		}
		else if(c==':')
		{
			//inicia un comando
			s=":";
		}
		else
		{
			s=s+c; //agrego el caracter a la cadena
		}		
	}
	while(1);
	return "";
}
//convierte la cadena en hexadeial a su valor entero de 8 bits
byte StringToByte(String s)//ok
{
	char b[10];
	s.toCharArray(b,10);
	long n = strtol (b, NULL, 16);
	return (byte)n;
}
//interpreta la cadena y verifica si tiene el formato y separa sus partes
int InterpretaComando(String cadena)//ok
{
	String c="",p1="",p2="",p3="",p4="",p5="";
	int pos=0;
	int status=0;
	int max=cadena.length();
	//region separar la cadena en sus componentes basicos
	for(pos=0;pos<max;pos++)
	{
    if(cadena[pos]==';')
    {
      break; //rompo el for porque encontre el final del comando
    }
		switch(status)
		{
			case 0: //verificar si comienza con :
				if(cadena[pos]!=':')
				{
					return 0; //no es cadena valida
				}
				status=1;
			break;
			case 1: //obtencion del comando
				if(cadena[pos]==',')
				{
					status=2;
				}
				else
				{
					c=c+cadena[pos];
				}
			break;
			case 2: //otencion del primer parametro
				if(cadena[pos]==',')
				{
					status=3;
				}
				else
				{
					p1=p1+cadena[pos];
				}
			break;
			case 3: //otencion del segundo parametro
				if(cadena[pos]==',')
				{
					status=4;
				}
				else
				{
					p2=p2+cadena[pos];
				}
			break;
			case 4: //otencion del tercer parametro
				if(cadena[pos]==',')
				{
					status=5;
				}
				else
				{
					p3=p3+cadena[pos];
				}
			break;
			case 5: //otencion del cuarto parametro
				if(cadena[pos]==',')
				{
					status=6;
				}
				else
				{
					p4=p4+cadena[pos];
				}
			break;
			case 6: //otencion del quinto parametro
				if(cadena[pos]==',')
				{
					status=7;
				}
				else
				{
					p5=p5+cadena[pos];
				}
			break;
		}		
	}
	//endregion
	//veo si el comenado es valido
	if(c!="D"&&c!="WP"&&c!="RP"&&c!="WD"&&c!="RD"&&c!="C"&&c!="WC")
	{
		return 0; //no es un comando valido
	}
	Comando=c;
	//asigno los parametros. Los parametros vienen en dos digitos que representan el valor de un byte en hexadecimal
	if(p1!="")
	{
		Parametro1=StringToByte(p1);
	}
	if(p2!="")
	{
		Parametro2=StringToByte(p2);
	}
	if(p3!="")
	{
		Parametro3=StringToByte(p3);
	}
	if(p4!="")
	{
		Parametro4=StringToByte(p4);
	}
	if(p5!="")
	{
		Parametro5=StringToByte(p5);
	}
	return 1;
}
//convierte un entero a su equivalente en cadena en exadecimal
String HexToString(char c)//ok
{
	String s=String(c,HEX);
	if(s.length()==1)
	{
		s="0"+s;
	}
	if(s.length()>2)
	{
		s=s.substring(s.length()-2);
	}
	return s;
}
//verifica que comando es y lo ejecuta
void EjecutaComando()
{
	char c;
	//Comandos
	//D-> Borrara chip
	//WP-> Escribir memoria de programa
	//RP-> Leer memoria de programa
	//WD-> Escribir en EEPROM
	//RD-> Leer EEPROM
	//C-> Gabara palabra de configuracion
	if(Comando=="D")
	{
		//hay que borrar el chip completo
		ErraceChip();
		//mando una respuesta
		Serial.println("<1>");
	}
	if(Comando=="WP")
	{
		//mando a grabar los dato que son dos bytes
		WriteProgramMemory(Parametro1, Parametro2, Parametro3, Parametro4,Parametro5);
		Serial.println("<1>");
	}
	if(Comando=="WC") //grabar configuracion 
	{
		//mando a grabar los dato que son dos bytes
		WriteConfigMemory(Parametro1, Parametro2, Parametro3, Parametro4,Parametro5);
		Serial.println("<1>");
	}
	if(Comando=="RP")
	{
    //TimeDelayPulce=50;
		c=ReadProgramMemory(Parametro1,Parametro2,Parametro3);
		Serial.println("<"+HexToString(c)+">");
	}
	if(Comando=="WD")
	{
		WriteDataMemory(Parametro1, Parametro2, Parametro3);
		Serial.println("<1>");
	}
	if(Comando=="RD")
	{
		c=ReadDataMemory(Parametro1, Parametro2);
		Serial.println("<"+HexToString(c)+">");
	}
	if(Comando=="C")
	{
		WriteConfigurationBits(Parametro1, Parametro2);
		Serial.println("<1>");
	}
}
//endregion
//region Loop
void loop()
{
//  Serial.println("Esperando comando");
	String s=RecibeComando();
// Serial.println("Cadena="+s);
  if(InterpretaComando(s)==0)
  {
    Serial.println("Comando no valido: "+s);
  }
  else
  {
    EjecutaComando();
  }
}
//endregion
