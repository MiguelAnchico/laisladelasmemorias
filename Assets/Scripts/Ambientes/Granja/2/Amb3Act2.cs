using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amb3Act2 : ExperienceController
{
    [SerializeField] List<ScriptableActivitiesData.ActivityData> toolsRound = new();
    [SerializeField] int _currentTool = 0;
    [SerializeField] int _isCorrect = 0;
    ScriptableActivitiesData.ActivityData toolEscogida;
    [SerializeField] CardControler CardTool;
     [SerializeField] CardControler _area1, _area2;
    [SerializeField] GameObject FB_Positive, FB_Negative, FB_Absurd, FB_End;
    public override void Start()
    {
        base.Start(); //llamada obligatoria al comenzar
        Debug.Log(Manager.GetManager<InputManager>().name);

        //iniciamos la lista de los animales del Data
        DesactiveInputManager(); //desactivamos el input manager por seguridad
        toolsRound = new(ActivityData.Data); //hacemos una copia de los datos del scriptable
        CombinarListaAleatorio(toolsRound, ActivityData.AbsurdData); //combinamos nuestra copia de la lista con los datos de absurdos (no necesitamos shadowcopy porque el resultaod se guardará en animalRound (primer parametro))
        RevolverLista(toolsRound); //revolvemos aleatoriamente todo
    }

    public void IniciarPartida(){
        //seleccionamos la primera herramienta de la lista
        toolEscogida = toolsRound[0];

        //verificamos si es absurdo
        if(toolEscogida.IsAbsurd)
        {
            // Si un # aleatorio es mayor que el porcentaje de absurdo de los datos de actividad entonces descartar este absurdo
            if(Random.Range(0, 100)>ActivityData.AbsurdPercent){
            toolsRound.RemoveAt(0); //pasamos al siguiente animal (no usamos el absurdo)
            IniciarPartida(); //volvemos a usar este metodo (recursividad)
            return;
            }

            // Si pasa el filtro inciar tiempo de espera
            StartAbsurdCorutine(()=>{
                _currentTool = 0;
                toolsRound.RemoveAt(0);
                ComeNewTool();
                FB_Positive.SetActive(true);
            });
        }
        EsperaYRealiza(.5f, ()=>{
            // Despues de verificar el animal a usar, asignamos sus valores a la carta
            CardTool.SetGameData(toolEscogida);
            CardTool.SetOnlyText(toolEscogida.Name);
            _isCorrect = Random.Range(1, 2);
            string aleatoryText = toolsRound[Random.Range(1, toolsRound.Count-1)].Text;
            _area1.SetOnlyText(_isCorrect==1?toolEscogida.Text:aleatoryText);
            _area2.SetOnlyText(_isCorrect==2?toolEscogida.Text:aleatoryText);
            }
        );
        CardTool.FlipCard(); 
    }
    public void UserSelect(int area){
        if(_isCorrect==0){return;} //sale si no hay herramietnas seleccionadas seleccionados

        //si es el ultimo animal
        if(toolsRound.Count==1)
        {
            //TERMINA
            return;
        }

        //si es una situacion absurda
        if(toolEscogida.IsAbsurd){
            FB_Absurd.SetActive(true);
            RestartAbsurdCorutine(()=>{
                //CORRECTO
            });
            return;
        }


        //verifica que el IsCorrect sea la zona seleccionada
        if(_isCorrect==area)
        {
            _isCorrect = 0;
            toolsRound.RemoveAt(0);
            ComeNewTool();
            FB_Positive.SetActive(true);

        }else{
            FB_Negative.SetActive(true);
        }
    }
    public void ComeNewTool()
    {
        CardTool.FlipCard();
        IniciarPartida();
    }

    public void Finalizar(){
        EndExperience();
    }
}
