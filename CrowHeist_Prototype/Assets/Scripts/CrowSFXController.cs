//using system.collections;
//using system.collections.generic;
//using unityengine;

//public class crowsfxcontroller : monobehaviour
//{
//    //[serializefield] private sfxdata[] _sfx;
//    //private audiosource _audiosource;
//    //private dictionary<string, sfxdata> _sfxdictionary = new dictionary<string, sfxdata>();

//    //private void awake()
//    //{
//    //    _audiosource = getcomponentinparent<audiosource>();
//    //    foreach (var sfx in _sfx)
//    //    {
//    //        _sfxdictionary.add(sfx.name, sfx);
//    //    }
//    //}

//    //public void playsfxoneshot(string sfxname)
//    //{
//    //    if (_sfxdictionary.containskey(sfxname))
//    //    {
//    //        _audiosource.playoneshot(_sfxdictionary[sfxname].sfx);
//    //    }else
//    //    {
//    //        debug.logwarning("sfx name not found");
//    //    }
//    //}

//    //public void playsfxwithparameters(string sfxname)
//    //{
//    //    float pitch = random.range(_sfxdictionary[sfxname].minpitch, _sfxdictionary[sfxname].maxpitch);
//    //    float volume = random.range(_sfxdictionary[sfxname].minvolume, _sfxdictionary[sfxname].maxvolume);

//    //    _audiosource.pitch = pitch;

//    //    if (_sfxdictionary.containskey(sfxname))
//    //    {
//    //        _audiosource.clip = _sfxdictionary[sfxname].sfx;
//    //        _audiosource.playoneshot(_sfxdictionary[sfxname].sfx, volume);
//    //    }else
//    //    {
//    //        debug.logwarning("sfx name not found");
//    //    }

//    //    _audiosource.pitch = 1;
//    //}
//}
