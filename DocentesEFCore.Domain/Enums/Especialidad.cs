using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Domain.Enums
{
    public enum Especialidad
    {
        [Description("Ingeniería Civil")]
        IC = 1,
        [Description("Ingeniería Eléctrica")]
        IE = 2,
        [Description("Ingeniería Mecánica")]
        IM = 3,
        [Description("Ingeniería Química")]
        IQ = 4,
        [Description("Ingeniería en Sistemas de Información")]
        ISI = 5,
        [Description("Tradicionales")]
        Tradicionales = 6,        
        Todas = 7 // ver como se maneja para guardar en la db
    }
}
