using CarritoCompras.Modelo;
using CarritoCompras.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarritoCompras.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageBolsa : ContentPage
	{
        string vtotal = "";
        public ObservableCollection<Bolsa> oListaBolsa { get; set; }
		public PageBolsa ()
		{
			InitializeComponent ();
            ObtenerBolsa();
            
        }

        private async void ObtenerBolsa()
        {
            Dictionary<string, Bolsa> oObjecto = await ApiServiceFirebase.ObtenerCarrito();

            if (oObjecto != null)
            {
                ObservableCollection<Bolsa> oListaTemp = new ObservableCollection<Bolsa>();
                if (oObjecto.Count > 0)
                {
                    foreach (KeyValuePair<string, Bolsa> item in oObjecto)
                    {
                        oListaTemp.Add(new Bolsa()
                        {
                            idbolsa = item.Key,
                            cantidad = item.Value.cantidad,
                            categoria = item.Value.categoria,
                            producto = item.Value.producto,
                            montoTotal = item.Value.cantidad * item.Value.producto.precio
                        });
                    }
                    oListaBolsa = oListaTemp;
                }

                ListViewBolsa.ItemsSource = oListaBolsa;
                sumarTotal();
            }
            else
                btnContinuar.IsEnabled = false;
            
        }

        private void TapMas_Tapped(object sender, EventArgs e)
        {
            var tt = sender;
            var tttt = tt.GetType();
            string test = tt.GetType().ToString();
            StackLayout oObjeto = (StackLayout)sender;
            oListaBolsa[0].cantidad = 10;
            oListaBolsa[0].producto.nombre = "xxxx";
            ListViewBolsa.ItemsSource = null;
            ListViewBolsa.ItemsSource = oListaBolsa;
        }



        private async void BtnEliminar_Clicked(object sender, EventArgs e)
        {
           string idbolsaSeleccionado = (sender as Button).CommandParameter.ToString();

            var DisplayResultado = await DisplayAlert("Notificacion", "Desea elimnar el producto del carrito", "Si", "No");
            if (DisplayResultado)
            {
                bool respuesta = await ApiServiceFirebase.RetirardeBolsa(idbolsaSeleccionado);
                if (respuesta)
                {
                    int index = 0;
                    foreach (Bolsa objeto in oListaBolsa)
                    {
                        if (objeto.idbolsa == idbolsaSeleccionado)
                        {
                            break;
                        }
                        index += 1;
                    }
                    oListaBolsa.RemoveAt(index);
                    ListViewBolsa.ItemsSource = null;
                    ListViewBolsa.ItemsSource = oListaBolsa;
                    sumarTotal();
                    await DisplayAlert("Notificacion", "Producto eliminado del carrito", "ok");
                }
                else
                    await DisplayAlert("Notificacion", "Hubo un problema al retirnar el producto", "ok");
            }
        }

        private void BtnMenos_Clicked(object sender, EventArgs e)
        {
            int index = 0;
            Bolsa oBolsa = (Bolsa)(sender as Button).CommandParameter;
            int cantidad = oBolsa.cantidad;
            if (cantidad > 1)
            {
                cantidad -= 1;
            }

            foreach(Bolsa objeto in oListaBolsa)
            {
                if(objeto.idbolsa == oBolsa.idbolsa)
                {
                    break;
                }
                index += 1;
            }
            oListaBolsa[index].cantidad = cantidad;
            oListaBolsa[index].montoTotal = cantidad * oListaBolsa[index].producto.precio;
            ListViewBolsa.ItemsSource = null;
            ListViewBolsa.ItemsSource = oListaBolsa;
            sumarTotal();
        }

        private void BtnMas_Clicked(object sender, EventArgs e)
        {
            int index = 0;
            Bolsa oBolsa = (Bolsa)(sender as Button).CommandParameter;
            int cantidad = oBolsa.cantidad;
            cantidad += 1;

            foreach (Bolsa objeto in oListaBolsa)
            {
                if (objeto.idbolsa == oBolsa.idbolsa)
                {
                    break;
                }
                index += 1;
            }
            oListaBolsa[index].cantidad = cantidad;
            oListaBolsa[index].montoTotal = cantidad * oListaBolsa[index].producto.precio;
            ListViewBolsa.ItemsSource = null;
            ListViewBolsa.ItemsSource = oListaBolsa;
            sumarTotal();
        }

        private void sumarTotal()
        {
            float total = 0;
            foreach (Bolsa objeto in oListaBolsa)
            {
                total = objeto.montoTotal + total;
            }

            lblMontoTotal.Text = string.Format("$ {0}", total);
            vtotal = string.Format("$ {0}", total);

        }

        private async void BtnContinuar_Clicked(object sender, EventArgs e)
        {
            bool delivery = false;
            string actionSheet = await DisplayActionSheet("Seleccione el metodo de entrega", "Cancelar", null, "Envio a domicilio", "Retiro en el restaurante");
            


            switch (actionSheet)
            {
                case "Envio a domicilio":
                    delivery = true;
                    await Navigation.PushAsync(new PageDespacho(oListaBolsa, delivery));
                    break;
                case "Retiro en el restaurante":
                    delivery = false;
                    await Navigation.PushAsync(new PageDespacho(oListaBolsa, delivery));
                    break;
                default:
                    break;
            }

            
        }
    }
        
}